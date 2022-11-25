namespace BankAccount.Shared.QueueServices
{
    public interface IQueueService
    {
        Task PublishMessageToQueue(string queueName, string message);
    }
}