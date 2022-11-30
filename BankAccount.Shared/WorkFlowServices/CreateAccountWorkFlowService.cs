using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain.Entities;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class CreateAccountWorkFlowService : IWorkflowService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateAccountWorkFlowService> _logger;
        private readonly IRepository<Account> _createAccountPayloadRepository;
        private readonly IRepository<CreditScore> _creditScoreRepository;
        private readonly IQueueService _queueService;

        public CreateAccountWorkFlowService(
            IMapper mapper,
            ILogger<CreateAccountWorkFlowService> logger,
            IRepository<Account> createAccountPayloadRepository,
            IRepository<CreditScore> creditScoreRepository,
            IQueueService queueService)
        {
            _logger = logger;
            _mapper = mapper;
            _createAccountPayloadRepository = createAccountPayloadRepository;
            _creditScoreRepository = creditScoreRepository;
            _queueService = queueService;
        }

        public WorkFlow WorkFlow => WorkFlow.CreateAccount;

        public OperationResult<string> ValidateMetadata(string metadata)
        {
            try
            {
                CreateAccountPayload? model = JsonConvert.DeserializeObject<CreateAccountPayload>(metadata);
                if (string.IsNullOrWhiteSpace(model.Email))
                    return OperationResult<string>.Failed($"{nameof(model.Email)} is required");

                if (string.IsNullOrWhiteSpace(model.FirstName))
                    return OperationResult<string>.Failed($"{nameof(model.FirstName)} is required");

                if (string.IsNullOrWhiteSpace(model.LastName))
                    return OperationResult<string>.Failed($"{nameof(model.LastName)} is required");

                if (!Validation.IsValidEmail(model.Email))
                    return OperationResult<string>.Failed($"Invalid {nameof(model.Email)}");

                return OperationResult<string>.Success;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"Unable to deserialize metadata errors:{ex.Message}");
                return OperationResult<string>.Failed();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"An error occured during data processing {ex}");
                throw;
            }
        }

        public async Task ExecuteAsync(string metadata, string sessionId)
        {
            try
            {
                var validateMetdata = ValidateMetadata(metadata);
                if (!validateMetdata.Successful)
                {
                    _logger.LogWarning($"Metadata validation was unsuccessful with error: {validateMetdata.Result}");
                }

                CreateAccountPayload model = JsonConvert.DeserializeObject<CreateAccountPayload>(metadata);

                _logger.LogDebug("Initiating Call to database to store Create Account Payload information");

                var entity = _mapper.Map<Account>(model);
                entity.SessionId = sessionId;
                entity.AccountNumber = Helper.RandomDigits();

                var creditScore = await _creditScoreRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (creditScore != null)
                {
                    entity.CreditScore = creditScore.Score;
                    entity.SocialSecurityNumber = creditScore.SocialSecurityNumber;
                }

                await _createAccountPayloadRepository.InsertAsync(entity);
                await _createAccountPayloadRepository.DbContext.SaveChangesAsync();

                _logger.LogDebug("Sucessfully created account");

                var metadataForEmail = new CommunicateWithMemberPayload(entity.AccountNumber, entity.CreditScore, entity.Email,
                    $"{entity.FirstName} {entity.LastName}");

                var serializedMetadata = JsonConvert.SerializeObject(metadataForEmail);

                _logger.LogDebug("Initiating sending data to the queue");
                await _queueService.PublishMessageToQueue(WorkFlow.CreateAccount.ToString(), serializedMetadata, sessionId);
                _logger.LogDebug("Data sent to queue sucessfully and workflow completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
