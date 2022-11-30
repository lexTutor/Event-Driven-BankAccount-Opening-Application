namespace BankAccount.Shared.CustomExceptions
{
    public class QueueServiceException : Exception
    {
        public QueueServiceException(string? message) : base(message)
        {
        }
    }
}
