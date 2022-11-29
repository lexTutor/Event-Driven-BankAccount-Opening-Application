using BankAccount.Shared.Contracts;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Logging;
using static BankAccount.Shared.Domain.RecordTypes;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.Shared.OrchestorService
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly IWorkflowProviderSelector _workflowProvider;
        private readonly IQueueService _queueService;
        private readonly ILogger<OrchestratorService> _logger;
        public OrchestratorService(IWorkflowProviderSelector workflowProvider,
            IQueueService queueService,
            ILogger<OrchestratorService> logger)
        {
            _workflowProvider = workflowProvider;
            _logger = logger;
            _queueService = queueService;
        }
        public async Task<OperationResult<string>> InitiateWorkFlow(InitiateWorkFlowPayload payload)
        {
            _logger.LogInformation($"Initiating Workflow for WorkFlowId :{payload.WorkFlowId}");

            WorkFlow workFlowType = (WorkFlow)payload.WorkFlowId;

            IWorkflowService workFlow = _workflowProvider.GetWorkFlowService(workFlowType);
            if (workFlow == null)
                return OperationResult<string>.Failed($"Invalid {nameof(payload.WorkFlowId)}");

            OperationResult<string> validationResult = workFlow.ValidateMetadata(payload.Metadata);
            if (!validationResult.Successful)
                return OperationResult<string>.Failed($"Invalid {nameof(payload.Metadata)} for Workflow {workFlowType}");

            _logger.LogInformation($"Publishing data to queue {workFlowType}");
            await _queueService.PublishMessageToQueue(workFlowType.ToString(), payload.Metadata);
            _logger.LogInformation($"Published data to queue {workFlowType}");

            return new OperationResult<string>() { Result = "Workflow successfully initiated", Successful = true };
        }
    }
}
