using AutoMapper;
using BankAccount.Shared.Contracts;
using BankAccount.Shared.Data;
using BankAccount.Shared.OrchestorService;
using BankAccount.Shared.QueueServices;
using BankAccount.Shared.Services;
using BankAccount.Shared.Utilities;
using BankAccount.Shared.WorkFlowServices;
using Mailjet.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccount.Shared
{
    public static class ServiceCollectionExtension
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<BankContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrchestratorService, OrchestratorService>();
            services.AddScoped<IWorkflowService, PotentialMemberWorkflowService>();
            services.AddScoped<IWorkflowService, CreateAccountWorkFlowService>();
            services.AddScoped<IWorkflowService, CommunicateWithMemberWorkflowService>();
            services.AddScoped<IWorkflowProviderSelector, WorkFlowProviderSelector>();
            services.AddScoped<IReferenceNumberService, ReferenceNumberService>();
            services.AddScoped<IDistributedLockManager, DistributedLockManager>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            #region ConfigVars

            var mailJetConfig = new MailjetClient
                (
                configuration["MailJetSettings:PublicKey"],
                configuration["MailJetSettings:PrivateKey"]
                );

            #endregion

            services.AddSingleton(mailJetConfig);

            return services;
        }

        public static void AddAutoMap(this IServiceCollection services)
        {
            IMapper mapper = AutoMapperConfiguration.ConfigureMappings();
            services.AddSingleton(mapper);
        }
    }
}
