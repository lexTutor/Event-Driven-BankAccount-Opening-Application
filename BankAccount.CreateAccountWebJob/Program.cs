using BankAccount.CreateAccountWebJob;
using BankAccount.Shared;
using BankAccount.Shared.WebJobConfiguration;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

class Program
{
    static async Task Main()
    {
        var builder = new HostBuilder();
        builder.ConfigureAppConfiguration((builder) =>
        {
            builder.AddEnvironmentVariables()
                   .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                   .Build();
        });

        builder.ConfigureWebJobs(b =>
        {
            b.AddServiceBus(sbOptions =>
            {
                sbOptions.AutoCompleteMessages = true;
                sbOptions.MaxConcurrentCalls = 8;
            });
        });
        builder.ConfigureLogging((context, b) =>
        {
            b.AddConsole();

        }).ConfigureServices((context, services) =>
        {
            services.AddLogging();
            services.AddSingleton<IJobActivator, WebJobActivator>();
            services.AddScoped<CreateAccountWebJob, CreateAccountWebJob>();
            services.AddSharedServices(context.Configuration);
            services.AddAutoMap();
            services.AddDatabase(context.Configuration);
        });
        var host = builder.Build();
        using (host)
        {
            await host.RunAsync();
        }
    }
}
