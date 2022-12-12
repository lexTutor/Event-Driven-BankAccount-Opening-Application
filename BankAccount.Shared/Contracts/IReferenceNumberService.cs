namespace BankAccount.Shared.Contracts
{
    public interface IReferenceNumberService
    {
        /// <summary>
        /// Increments reference number meeting the supplied criteria and returns a range of reference ids acquired (minimum and maximum values).
        /// </summary>
        /// <param name="agencyId"></param>
        /// <param name="dimension"></param>
        /// <param name="increament"></param>
        /// <returns>Returns a range of acquired reference identifiers (minimum and maximum values)</returns>
        Task<(long, long)> IncreamentNextValAsync(string dimension, int increament = 1);
    }
}
