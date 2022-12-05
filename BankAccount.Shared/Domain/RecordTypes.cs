using System.ComponentModel.DataAnnotations;

namespace BankAccount.Shared.Domain
{
    public class RecordTypes
    {
        public record InitiateWorkFlowPayload([Required] int WorkFlowId, [Required] string Metadata, string SessionId);

        public record PotentialMemberPayload([Required] string WebsiteStartingUrl, [Required] string IpAddress, [Required] DateTime InitializationTime);

        public record CreateAccountPayload([Required] string Email, [Required] string FirstName, [Required] string LastName);

        public record CommunicateWithMemberPayload(string AccountNumber, int? CreditScore, string Email, string FullName);

        public record EnumModel(int Id, string Name);
    }
}
