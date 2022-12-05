using BankAccount.Shared.Contracts;
using BankAccount.Shared.Data;
using Medallion.Threading;
using Medallion.Threading.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankAccount.Shared.Services
{
    public class DistributedLockManager : IDistributedLockManager
    {
        private const string CONNECTIONSTRING_KEY = "DefaultConnection";
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public DistributedLockManager(
          IConfiguration configuration,
          ILogger<DistributedLockManager> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IDistributedSynchronizationHandle> AcquireLockAsync(string name, TimeSpan? timeout = default, CancellationToken cancellationToken = default)
        {
            var connectionString = _configuration.GetConnectionString(CONNECTIONSTRING_KEY);

            var distributedLock = new SqlDistributedLock(name, connectionString);
            var timeoutTimeSpan = timeout ?? TimeSpan.Zero;

            _logger.LogDebug("Acquiring a lock on {LockName}", name);

            IDistributedSynchronizationHandle handle = await distributedLock.TryAcquireAsync(timeoutTimeSpan, cancellationToken);

            if (handle == null)
                return null;

            _logger.LogDebug("Lock acquired on {LockName}", name);

            return new FailsafeHandle(handle);
        }
    }
}
