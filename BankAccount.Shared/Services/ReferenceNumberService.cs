using BankAccount.Shared.Contracts;
using BankAccount.Shared.Data;
using BankAccount.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankAccount.Shared.Services
{
    public class ReferenceNumberService : IReferenceNumberService
    {
        private readonly BankContext _context;
        private readonly IRepository<ReferenceNumber> _refrenceNumberRepository;
        private readonly IDistributedLockManager _distributedLockManager;
        public ReferenceNumberService(
            IRepository<ReferenceNumber> refrenceNumberRepository,
            IDistributedLockManager distributedLockManager,
            BankContext context)
        {
            _context = context;
            _distributedLockManager = distributedLockManager;
            _refrenceNumberRepository = refrenceNumberRepository;
        }

        /// <inheritdoc cref="IReferenceNumberService.IncreamentNextValAsync(string, int)"/>
        public async Task<(long, long)> IncreamentNextValAsync(string dimension, int increament = 1)
        {
            if (increament <= 0)
                throw new InvalidOperationException("Reference number next val increament is invalid.");
            var normalizedDimension = dimension?.ToLower() ?? string.Empty;
            (long, long) referenceNumbersRange = (0, 0);

            using (var handle = await _distributedLockManager.AcquireLockAsync(nameof(IReferenceNumberService), TimeSpan.FromHours(1)))
            {
                int records = await _context.Database.ExecuteSqlRawAsync(@"
                                if not exists (select id from referenceNumbers where dimension = {0})
                                    begin
                                    insert into referenceNumbers(dimension,nextval) values({0}, 1 + {1})
                                    end
                                else
                                    begin
                                        update referenceNumbers set nextVal = nextVal + {1} where dimension = {0}
                                    end
                                ", normalizedDimension, increament, DateTime.UtcNow);

                if (records <= 0)
                    throw new InvalidOperationException("Reference number database update failure.");
                var newNextVal = await _refrenceNumberRepository.Table.AsNoTracking()
                    .Where(x => x.Dimension.ToLower() == normalizedDimension)
                    .Select(x => x.NextVal).FirstOrDefaultAsync();
                referenceNumbersRange = (newNextVal - increament, newNextVal - 1);
                return referenceNumbersRange;
            }
        }
    }
}
