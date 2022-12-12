using Medallion.Threading;

namespace BankAccount.Shared.Contracts
{
    public interface IDistributedLockManager
    {
        Task<IDistributedSynchronizationHandle> AcquireLockAsync(string name, TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);
    }
}
