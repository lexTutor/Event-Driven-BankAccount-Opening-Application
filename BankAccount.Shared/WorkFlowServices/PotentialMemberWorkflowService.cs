using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Newtonsoft.Json;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class PotentialMemberWorkflowService : IWorkflowService
    {
        public WorkFlow WorkFlow => WorkFlow.PotentialMember;

        public OperationResult<string> ValidateMetadata(string metadata)
        {
            try
            {
                PotentialMemberPayload model = JsonConvert.DeserializeObject<PotentialMemberPayload>(metadata);

                if (string.IsNullOrWhiteSpace(model.WebsiteStartingUrl))
                    return OperationResult<string>.Failed($"{nameof(model.WebsiteStartingUrl)} is required");

                if (model.TOD <= DateTime.UtcNow)
                    return OperationResult<string>.Failed($"Invalid {nameof(model.TOD)}");

                return OperationResult<string>.Success;
            }
            catch (JsonSerializationException)
            {
                return OperationResult<string>.Failed();
            }
        }
    }
}
