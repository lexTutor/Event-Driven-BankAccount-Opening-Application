using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Shared.Domain.Entities
{
    public class ReferenceNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Dimension { get; set; }
        public long NextVal { get; set; }
    }
}
