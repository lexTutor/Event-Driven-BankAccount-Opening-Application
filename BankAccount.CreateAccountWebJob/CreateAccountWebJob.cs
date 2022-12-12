using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.CreateAccountWebJob
{
    public class CreateAccountWebJob
    {
        private readonly IWorkflowService _createAccountWorkFlow;
        private readonly ILogger<CreateAccountWebJob> _logger;

        public WorkFlow WorkFlow => WorkFlow.CreateAccount;
        public CreateAccountWebJob(IEnumerable<IWorkflowService> orchestrators,
            ILogger<CreateAccountWebJob> logger)
        {
            _createAccountWorkFlow = orchestrators.FirstOrDefault(x => x.WorkFlow == WorkFlow)
                ?? throw new ArgumentNullException(nameof(orchestrators));
            _logger = logger;
        }

        [FunctionName("CreateAccountFuction")]
        public async Task Run([ServiceBusTrigger(queueName: Constants.CreateAccountQueue)] string myQueueItem, string sessionId)
        {
            try
            {
                _logger.LogDebug($"ServiceBus queue trigger function attempting to process message {myQueueItem}");

                await _createAccountWorkFlow.ExecuteAsync(myQueueItem, sessionId);

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
