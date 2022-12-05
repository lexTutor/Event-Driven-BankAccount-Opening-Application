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
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<CreditScore> _creditScoreRepository;
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly IQueueService _queueService;

        public CreateAccountWorkFlowService(
            IMapper mapper,
            ILogger<CreateAccountWorkFlowService> logger,
            IRepository<Account> accountRepository,
            IRepository<CreditScore> creditScoreRepository,
            IQueueService queueService,
            IReferenceNumberService referenceNumberService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _creditScoreRepository = creditScoreRepository ?? throw new ArgumentNullException(nameof(creditScoreRepository));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _referenceNumberService = referenceNumberService ?? throw new ArgumentNullException(nameof(referenceNumberService));
        }

        public WorkFlow WorkFlow => WorkFlow.CreateAccount;

        public OperationResult<dynamic> ValidateMetadata(string metadata)
        {
            try
            {
                CreateAccountPayload? model = JsonConvert.DeserializeObject<CreateAccountPayload>(metadata);
                if (string.IsNullOrWhiteSpace(model.Email))
                    return OperationResult<dynamic>.Failed($"{nameof(model.Email)} is required");

                if (string.IsNullOrWhiteSpace(model.FirstName))
                    return OperationResult<dynamic>.Failed($"{nameof(model.FirstName)} is required");

                if (string.IsNullOrWhiteSpace(model.LastName))
                    return OperationResult<dynamic>.Failed($"{nameof(model.LastName)} is required");

                if (!Validation.IsValidEmail(model.Email))
                    return OperationResult<dynamic>.Failed($"Invalid {nameof(model.Email)}");

                return new OperationResult<dynamic>
                {
                    Result = model,
                    Successful = true
                };
            }
            catch (JsonException ex)
            {
                _logger.LogWarning($"Unable to deserialize metadata errors:{ex.Message}");
                return OperationResult<dynamic>.Failed("Unable to deserialize metadata");
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
                    return;
                }

                CreateAccountPayload model = (CreateAccountPayload)validateMetdata.Result;

                _logger.LogDebug("Initiating Call to database to store Create Account Payload information");

                Account entity = _accountRepository.Table.FirstOrDefault(x => x.Email == model.Email);
                CommunicateWithMemberPayload metadataForEmail;
                if (entity != null)
                {
                    _logger.LogWarning($"User {model.Email} already has an account, details will be mailed to the user");

                    metadataForEmail = new CommunicateWithMemberPayload(entity.AccountNumber, entity.CreditScore,
                        entity.Email, $"{entity.FirstName} {entity.LastName}");

                    await _queueService.PublishMessageToQueue(WorkFlow.CommunicateWithMember.ToString(),
                        JsonConvert.SerializeObject(metadataForEmail), sessionId);

                    return;
                }

                entity = _mapper.Map<Account>(model);
                entity.SessionId = sessionId;
                entity.AccountNumber = await ReferenceNumberServiceExtensions.GetAccountNumber(_referenceNumberService);

                var creditScore = await _creditScoreRepository.TableNoTracking.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (creditScore != null)
                {
                    entity.CreditScore = creditScore.Score;
                    entity.SocialSecurityNumber = creditScore.SocialSecurityNumber;
                }

                await _accountRepository.InsertAsync(entity);
                await _accountRepository.DbContext.SaveChangesAsync();

                _logger.LogDebug("Sucessfully created account");

                metadataForEmail = new CommunicateWithMemberPayload(entity.AccountNumber, entity.CreditScore, entity.Email,
                    $"{entity.FirstName} {entity.LastName}");

                _logger.LogDebug("Initiating sending data to the queue");

                await _queueService.PublishMessageToQueue(WorkFlow.CommunicateWithMember.ToString(),
                    JsonConvert.SerializeObject(metadataForEmail), sessionId);

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
