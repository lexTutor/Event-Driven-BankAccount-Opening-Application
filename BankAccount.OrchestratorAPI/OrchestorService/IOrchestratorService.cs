using BankAccount.Shared.Domain;
using BankAccount.Shared.Utilities;

namespace BankAccount.OrchestratorAPI.OrchestorService
{
    public interface IOrchestratorService
    {
        Task<OperationResult<string>> InitiateWorkFlow(RecordTypes.InitiateWorkFlowPayload payload);
    }
}