using Azure.Messaging.ServiceBus;
using BankAccount.Shared.Utilities;

namespace BankAccount.Shared.QueueServices
{
    public class QueueService : IQueueService
    {
        public async Task PublishMessageToQueue(string queueName, string message)
        {
            await using ServiceBusClient client = new(connectionString: Connection.ConnectionString);

            await using ServiceBusSender serviceSender = client.CreateSender(queueName);

            await serviceSender.SendMessageAsync(new(message));
        }
    }

}
