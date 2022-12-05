using BankAccount.Shared.Contracts;

namespace BankAccount.Shared.Utilities
{
    public static class ReferenceNumberServiceExtensions
    {
        public static async Task<string> GetAccountNumber(this IReferenceNumberService referenceNumberService)
        {
            var range = await referenceNumberService.IncreamentNextValAsync(Constants.AccountNumberReferenceDimension.ToUpperInvariant());
            return range.Item1.ToString("0000000000");
        }
    }
}
