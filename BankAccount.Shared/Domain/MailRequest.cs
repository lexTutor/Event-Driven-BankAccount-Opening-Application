namespace BankAccount.Shared.Domain
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Copies { get; set; }
    }
}
