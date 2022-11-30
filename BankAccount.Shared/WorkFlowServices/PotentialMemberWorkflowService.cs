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
            _logger = logger;
            _potentialMemberRepository = potentialMemberRepository;
            _mapper = mapper;
        }

        public WorkFlow WorkFlow => WorkFlow.PotentialMember;

        public OperationResult<string> ValidateMetadata(string metadata)
        {
            try
            {
                PotentialMemberPayload model = JsonConvert.DeserializeObject<PotentialMemberPayload>(metadata);

                if (string.IsNullOrWhiteSpace(model.WebsiteStartingUrl))
                    return OperationResult<string>.Failed($"{nameof(model.WebsiteStartingUrl)} is required");

                if (string.IsNullOrWhiteSpace(model.IpAddress))
                    return OperationResult<string>.Failed($"{nameof(model.IpAddress)} is required");

                if (model.TOD > DateTime.UtcNow)
                    return OperationResult<string>.Failed($"Invalid {nameof(model.TOD)}");

                return OperationResult<string>.Success;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Unable to deserialize metadata errors:{ex.Message}");
                return OperationResult<string>.Failed();
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
                }

                PotentialMemberPayload model = JsonConvert.DeserializeObject<PotentialMemberPayload>(metadata);

                _logger.LogDebug("Initiating Call to database to store potential Member information");

                var entity = _mapper.Map<PotentialMember>(model);
                entity.SessionId = sessionId;

                await _potentialMemberRepository.InsertAsync(entity);
                await _potentialMemberRepository.DbContext.SaveChangesAsync();

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
