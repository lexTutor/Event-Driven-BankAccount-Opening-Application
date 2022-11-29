using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;

namespace BankAccount.Shared.WorkFlowServices
{
    public class CreateAccountWorkFlowService : IWorkflowService
    {
        private readonly ILogger<CreateAccountWorkFlowService> _logger;
        public CreateAccountWorkFlowService(ILogger<CreateAccountWorkFlowService> logger)
        {
            _logger = logger;
        }
        public Enumeration.WorkFlow WorkFlow => Enumeration.WorkFlow.CreateAccount;

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

        public Task ExecuteAsync(string metadata)
        {
            throw new NotImplementedException();
        }
    }
}
