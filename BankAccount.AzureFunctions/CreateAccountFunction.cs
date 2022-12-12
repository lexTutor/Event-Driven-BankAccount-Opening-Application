using BankAccount.Shared.Contracts;
using BankAccount.Shared.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BankAccount.Shared.Utilities.Enumeration;

namespace BankAccount.AzureFunctions
{
    public class CreateAccountFunction
    {
        private readonly IWorkflowService _createAccountWorkFlow;
        private readonly ILogger<CreateAccountFunction> _logger;

        public WorkFlow WorkFlow => WorkFlow.CreateAccount;

        public CreateAccountFunction(IEnumerable<IWorkflowService> orchestrators,
            ILogger<CreateAccountFunction> logger)
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
