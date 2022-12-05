using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.CommunicateWithMemberWebJob
{
    public class CommunicateWithMemberWebJob
    {
        private readonly IWorkflowService _communicateWithMemberWorkFlow;
        private readonly ILogger<CommunicateWithMemberWebJob> _logger;

        public WorkFlow WorkFlow => WorkFlow.CommunicateWithMember;
        public CommunicateWithMemberWebJob(IEnumerable<IWorkflowService> orchestrators,
            ILogger<CommunicateWithMemberWebJob> logger)
        {
            _communicateWithMemberWorkFlow = orchestrators.FirstOrDefault(x => x.WorkFlow == WorkFlow)
                ?? throw new ArgumentNullException(nameof(orchestrators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("CommunicateWithMemberFuction")]
        public async Task Run([ServiceBusTrigger(queueName: Constants.CommunicateWithMemberQueue)] string myQueueItem, string sessionId)
        {
            try
            {
                _logger.LogDebug($"ServiceBus queue trigger function attempting to process message {myQueueItem}");

                await _communicateWithMemberWorkFlow.ExecuteAsync(myQueueItem, sessionId);

                _logger.LogDebug($"ServiceBus queue trigger function processed message successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw; // This will enable the message to be re queued.
            }
        }
    }
}
