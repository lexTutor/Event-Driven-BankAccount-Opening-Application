using BankAccount.Shared.Utilities;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.Contracts
{
    public interface IWorkflowService
    {
        WorkFlow WorkFlow { get; }

        Task ExecuteAsync(string metadata, string sessionId);

        OperationResult<dynamic> ValidateMetadata(string metadata);
    }
}
