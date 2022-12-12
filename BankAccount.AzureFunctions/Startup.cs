using BankAccount.AzureFunctions;
using BankAccount.Shared;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BankAccount.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                             .AddEnvironmentVariables()
                             .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                             .Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddLogging();
            builder.Services.AddSharedServices(configuration);
            builder.Services.AddAutoMap();
            builder.Services.AddDatabase(configuration);
        }
    }
}
