namespace BankAccount.Shared.Domain.Entities
{
    public class CreditScore : BaseEntity
    {
        public int Score { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Email { get; set; }
    }
}
