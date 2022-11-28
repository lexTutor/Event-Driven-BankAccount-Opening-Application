namespace BankAccount.Shared.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public int CreditScore { get; set; }
    }
}
