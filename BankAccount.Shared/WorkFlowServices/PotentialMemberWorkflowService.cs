using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Domain.Entities;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class PotentialMemberWorkflowService : IWorkflowService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PotentialMemberWorkflowService> _logger;
        private readonly IRepository<PotentialMember> _potentialMemberRepository;
        public PotentialMemberWorkflowService(
            ILogger<PotentialMemberWorkflowService> logger,
            IRepository<PotentialMember> potentialMemberRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _potentialMemberRepository = potentialMemberRepository ?? throw new ArgumentNullException(nameof(potentialMemberRepository));
        }

        public WorkFlow WorkFlow => WorkFlow.PotentialMember;

        public OperationResult<dynamic> ValidateMetadata(string metadata)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(metadata)) 
                    return OperationResult<dynamic>.Failed($"Invalid {nameof(metadata)}");

                PotentialMemberPayload model = JsonConvert.DeserializeObject<PotentialMemberPayload>(metadata);

                if (string.IsNullOrWhiteSpace(model.WebsiteStartingUrl))
                    return OperationResult<dynamic>.Failed($"{nameof(model.WebsiteStartingUrl)} is required");

                if (string.IsNullOrWhiteSpace(model.IpAddress))
                    return OperationResult<dynamic>.Failed($"{nameof(model.IpAddress)} is required");

                if (model.InitializationTime > DateTime.UtcNow)
                    return OperationResult<dynamic>.Failed($"Invalid {nameof(model.InitializationTime)}");

                return new OperationResult<dynamic>
                {
                    Result = model,
                    Successful = true
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Unable to deserialize metadata errors:{ex.Message}");
                return OperationResult<dynamic>.Failed("Unable to deserialize metadata");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured during data processing {ex}");
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

                PotentialMemberPayload model = (PotentialMemberPayload)validateMetdata.Result;

                _logger.LogDebug("Initiating Call to database to store potential Member information");

                var entity = _mapper.Map<PotentialMember>(model);
                entity.SessionId = sessionId;

                await _potentialMemberRepository.SaveOrUpdateAsync(entity);

                _logger.LogDebug("Sucessfully stored potential member details");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured during data processing {ex}");
                throw;
            }
        }
    }
}
