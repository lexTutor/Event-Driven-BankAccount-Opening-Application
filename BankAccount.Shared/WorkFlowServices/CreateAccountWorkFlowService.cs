using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;

namespace BankAccount.Shared.WorkFlowServices
{
    public class CreateAccountWorkFlowService : IWorkflowService
    {
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
            catch (JsonSerializationException)
            {
                return OperationResult<string>.Failed();
            }
        }
    }
}
