using BankAccount.Shared.Utilities;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.Contracts
{
    public interface IWorkflowService
    {
        WorkFlow WorkFlow { get; }

        OperationResult<string> ValidateMetadata(string metadata);
    }
}
