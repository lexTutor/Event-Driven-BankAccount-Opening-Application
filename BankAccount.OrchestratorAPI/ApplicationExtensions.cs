using BankAccount.Shared.Data;

namespace BankAccount.OrchestratorAPI
{
    public static class ApplicationExtensions
    {
        public static void EnsureDatabaseSetup(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<BankContext>();
            db.Database.EnsureCreated();
        }
    }
}
