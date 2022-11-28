namespace BankAccount.Shared.Domain.Entities
{
    public class PotentialMember : BaseEntity
    {
        public string IPAddress { get; set; }
        public DateTimeOffset InitializationTime { get; set; }
        public string WebsiteLocation { get; set; }
    }
}
