using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccount.Shared.WebJobConfiguration
{
    public class WebJobActivator : IJobActivator
    {
        private readonly IServiceProvider _service;

        public WebJobActivator(IServiceProvider service)
        {
            _service = service;
        }

        public T CreateInstance<T>()
        {
            var service = _service.GetService<T>();
            return service;
        }
    }
}
