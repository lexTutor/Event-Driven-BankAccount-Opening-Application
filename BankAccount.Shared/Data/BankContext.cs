using BankAccount.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankAccount.Shared.Data
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        protected BankContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<CreditScore> CreditScores => Set<CreditScore>();
        public DbSet<PotentialMember> PotentialMembers => Set<PotentialMember>();
        public DbSet<ReferenceNumber> ReferenceNumbers => Set<ReferenceNumber>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetToStringConverter>(); // SqlLite workaround for DateTimeOffset sorting

            configurationBuilder
                .Properties<decimal>()
                .HaveConversion<double>(); // SqlLite workaround for decimal aggregations
        }
    }
}
