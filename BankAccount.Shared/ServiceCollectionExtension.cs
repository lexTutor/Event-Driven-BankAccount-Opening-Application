using BankAccount.Shared.Contracts;
using BankAccount.Shared.Data;
using BankAccount.Shared.OrchestorService;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.WorkFlowServices;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccount.Shared
{
    public static class ServiceCollectionExtension
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            var connectionString = "Data Source=BankAccount.db";

            services.AddSqlite<BankContext>(connectionString); ;
        }

        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IOrchestratorService, OrchestratorService>();
            services.AddScoped<IWorkflowService, PotentialMemberWorkflowService>();
            services.AddScoped<IWorkflowService, CreateAccountWorkFlowService>();
            services.AddScoped<IWorkflowService, CommunicateWithMemberWorkflowService>();
            services.AddScoped<IWorkflowProviderSelector, WorkFlowProviderSelector>();
            services.AddScoped<IQueueService, QueueService>();

            return services;
        }
    }
}
