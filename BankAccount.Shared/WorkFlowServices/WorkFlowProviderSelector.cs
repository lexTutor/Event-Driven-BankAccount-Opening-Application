using BankAccount.Shared.Contracts;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.WorkFlowServices
{
    public class WorkFlowProviderSelector : IWorkflowProviderSelector
    {
        private readonly IEnumerable<IWorkflowService> _orchestrators;

        public WorkFlowProviderSelector(IEnumerable<IWorkflowService> orchestrators)
        {
            _orchestrators = orchestrators ?? throw new ArgumentNullException(nameof(orchestrators));
        }

        public IWorkflowService GetWorkFlowService(WorkFlow workFlow)
            => _orchestrators.FirstOrDefault(o => o.WorkFlow == workFlow);
    }
}
