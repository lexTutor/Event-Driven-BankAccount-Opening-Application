using Medallion.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAccount.Shared.Data
{
    public class FailsafeHandle : IDistributedSynchronizationHandle
    {
        private readonly IDistributedSynchronizationHandle _handle;
        public FailsafeHandle(IDistributedSynchronizationHandle handle)
        {
            _handle = handle;
        }

        public CancellationToken HandleLostToken => _handle.HandleLostToken;

        public void Dispose()
        {
            try
            {
                _handle.Dispose();
            }
            catch
            {
                // Fail silently; if e.g the DB Connection failed, lock is lost anyway
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _handle.DisposeAsync();
            }
            catch
            {
                // Fail silently; if e.g the DB Connection failed, lock is lost anyway
            }
        }
    }
}
