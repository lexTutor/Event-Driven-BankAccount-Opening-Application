using BankAccount.Shared.Utilities;

namespace BankAccount.Shared.Contracts
{
    public interface IWorkflowProviderSelector
    {
        IWorkflowService GetWorkFlowService(Enumeration.WorkFlow workFlow);
    }
}