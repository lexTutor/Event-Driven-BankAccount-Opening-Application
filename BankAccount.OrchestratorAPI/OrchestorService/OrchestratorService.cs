using BankAccount.Shared.Contracts;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.Utilities;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.OrchestratorAPI.OrchestorService
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly IWorkflowProviderSelector _workflowProvider;
        private readonly IQueueService _queueService;

        public OrchestratorService(IWorkflowProviderSelector workflowProvider, IQueueService queueService)
        {
            _workflowProvider = workflowProvider;
            _queueService = queueService;
        }
        public async Task<OperationResult<string>> InitiateWorkFlow(InitiateWorkFlowPayload payload)
        {
            WorkFlow workFlowType = (WorkFlow)payload.WorkFlowId;

            IWorkflowService workFlow = _workflowProvider.GetWorkFlowService(workFlowType);
            if (workFlow == null)
                return OperationResult<string>.Failed($"Invalid {nameof(payload.WorkFlowId)}");

            OperationResult<string> validationResult = workFlow.ValidateMetadata(payload.Metadata);
            if (!validationResult.Successful)
                return OperationResult<string>.Failed($"Invalid {nameof(payload.Metadata)} for Workflow {workFlowType}");

            await _queueService.PublishMessageToQueue(workFlowType.ToString(), payload.Metadata);

            return new OperationResult<string>() { Result = "Workflow successfully initiated" };
        }
    }
}
