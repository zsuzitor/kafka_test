
using Microsoft.Extensions.DependencyInjection;

namespace KafkaTestCore.Models
{
    public static class ServiceLocator
    {
        public static IServiceProvider Provider { get; private set; }
        public static void Init(IServiceProvider provider)
        {
            Provider = provider;

        }

        public static IServiceProvider GetScope()
        {
            return Provider.CreateScope().ServiceProvider;
        }
        //public static ServiceProvider GetProvider()
        //{

        //}
    }
}
