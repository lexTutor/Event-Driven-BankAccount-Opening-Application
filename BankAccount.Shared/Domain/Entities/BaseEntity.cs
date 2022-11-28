namespace BankAccount.Shared.Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
