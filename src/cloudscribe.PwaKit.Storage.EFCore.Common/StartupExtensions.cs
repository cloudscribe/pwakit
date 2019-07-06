using cloudscribe.PwaKit.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace cloudscribe.PwaKit.Storage.EFCore.Common
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPwaStorageEFCommon(this IServiceCollection services)
        {
            services.AddSingleton<IPushSubscriptionStore, PushSubscriptionStore>();


            return services;
        }

    }
}
