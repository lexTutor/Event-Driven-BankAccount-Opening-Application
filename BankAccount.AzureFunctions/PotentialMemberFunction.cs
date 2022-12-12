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
    public class PotentialMemberFunction
    {
        private readonly IWorkflowService _potentialMemberWorkFlow;
        private readonly ILogger<PotentialMemberFunction> _logger;

        public WorkFlow WorkFlow => WorkFlow.PotentialMember;
        public PotentialMemberFunction(IEnumerable<IWorkflowService> orchestrators,
            ILogger<PotentialMemberFunction> logger)
        {
            _potentialMemberWorkFlow = orchestrators.FirstOrDefault(x => x.WorkFlow == WorkFlow)
                ?? throw new ArgumentNullException(nameof(orchestrators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [FunctionName("PotentialMemberFuction")]
        public async Task Run([ServiceBusTrigger(queueName: Constants.PotentialMemberQueue)] string myQueueItem, string sessionId)
        {
            try
            {
                _logger.LogDebug($"ServiceBus queue trigger function attempting to process message {myQueueItem}");

                await _potentialMemberWorkFlow.ExecuteAsync(myQueueItem, sessionId);

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
