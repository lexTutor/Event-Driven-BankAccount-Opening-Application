using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class CommunicateWithMemberWorkflowService : IWorkflowService
    {
        private readonly ILogger<CommunicateWithMemberWorkflowService> _logger;

        public CommunicateWithMemberWorkflowService(ILogger<CommunicateWithMemberWorkflowService> logger)
        {
            _logger = logger;
        }

        public WorkFlow WorkFlow => WorkFlow.CommunicateWithMember;
       
        public OperationResult<string> ValidateMetadata(string metadata)
        {
            try
            {
                CommunicateWithMemberPayload model = JsonConvert.DeserializeObject<CommunicateWithMemberPayload>(metadata);

                if (string.IsNullOrWhiteSpace(model.AccountNumber))
                    return OperationResult<string>.Failed($"{nameof(model.AccountNumber)} is required");

                if (string.IsNullOrWhiteSpace(model.Email))
                    return OperationResult<string>.Failed($"{nameof(model.Email)} is required");

                if (string.IsNullOrWhiteSpace(model.FullName))
                    return OperationResult<string>.Failed($"{nameof(model.FullName)} is required");

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

        public Task ExecuteAsync(string metadata)
        {
            throw new NotImplementedException();
        }

    }
}
