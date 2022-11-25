using BankAccount.Shared.Contracts;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.WorkFlowServices;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccount.Shared
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowService, PotentialMemberWorkflowService>();
            services.AddScoped<IWorkflowService, CreateAccountWorkFlowService>();
            services.AddScoped<IWorkflowService, CommunicateWithMemberWorkflowService>();
            services.AddScoped<IWorkflowProviderSelector, WorkFlowProviderSelector>();
            services.AddScoped<IQueueService, QueueService>();

            return services;
        }
    }
}
