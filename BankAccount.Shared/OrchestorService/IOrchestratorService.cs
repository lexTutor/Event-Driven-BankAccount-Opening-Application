using BankAccount.Shared.Domain;
using BankAccount.Shared.Utilities;

namespace BankAccount.Shared.OrchestorService
{
    public interface IOrchestratorService
    {
        Task<OperationResult<string>> InitiateWorkFlow(RecordTypes.InitiateWorkFlowPayload payload);
    }
}