using Azure.Messaging.ServiceBus;
using BankAccount.Shared.CustomExceptions;
using BankAccount.Shared.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankAccount.Shared.QueueServices
{
    public class QueueService : IQueueService
    {
        private readonly ILogger<QueueService> _logger;
        private readonly IConfiguration _configuration;

        public QueueService(ILogger<QueueService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task PublishMessageToQueue(string queueName, string message, string sessionId)
        {
            try
            {
                await using ServiceBusClient client = new(connectionString: _configuration[Constants.ServiceBus]);

                await using ServiceBusSender serviceSender = client.CreateSender(queueName);

                ServiceBusMessage serviceBusMessage = new(message);
                serviceBusMessage.SessionId = sessionId;

                await serviceSender.SendMessageAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new QueueServiceException("Unable to upload data to queue");
            }
        }
    }

}
