namespace BankAccount.Shared.Domain
{
    public class RecordTypes
    {
        public record InitiateWorkFlowPayload(int WorkFlowId, string Metadata);

        public record PotentialMemberPayload(string WebsiteStartingUrl, DateTime TOD);

        public record CreateAccountPayload(string Email, string FirstName, string LastName);

        public record CommunicateWithMemberPayload(string AccountNumber, int CreditScore, string Email, string FullName);

        public record EnumModel(int Id, string Name);
    }
}
