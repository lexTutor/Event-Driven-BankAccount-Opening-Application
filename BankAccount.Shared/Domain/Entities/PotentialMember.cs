namespace BankAccount.Shared.Domain.Entities
{
    public class PotentialMember : BaseEntity
    {
        public string IPAddress { get; set; }
        public DateTimeOffset TOD { get; set; }
        public string WebsiteStartingUrl { get; set; }
    }
}
