namespace BankAccount.Shared.Utilities
{
    public class Constants
    {
        public const string InternalServerErrorMessage = "A server error occured, we are working on it";

        public const string ServiceBus = "ServiceBus";

        public const string PotentialMemberQueue = "PotentialMember";
        public const string CreateAccountQueue = "CreateAccount";
        public const string CommunicateWithMemberQueue = "CommunicateWithMember";
    }
}
