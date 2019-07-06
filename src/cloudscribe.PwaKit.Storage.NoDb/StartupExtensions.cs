using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Storage.NoDb;
using NoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPwaNoDbStorage(this IServiceCollection services)
        {

            services.AddNoDbSingleton<PushDeviceSubscription>();
            services.AddSingleton<IPushSubscriptionStore, PushSubscriptionStore>();


            return services;
        }

    }
}
