using System.ComponentModel.DataAnnotations;

namespace BankAccount.Shared.Domain.Entities
{
    public class Account : BaseEntity
    {
        [MaxLength(10)]
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public int? CreditScore { get; set; }
    }
}
