using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Storage.NoDb;
using NoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static PwaBuilder AddNoDbStorage(this PwaBuilder builder)
        {

            builder.Services.AddNoDbSingleton<cloudscribe.PwaKit.Models.PushDeviceSubscription>();
            builder.Services.AddSingleton<IPushSubscriptionStore, PushSubscriptionStore>();


            return builder;
        }

    }
}
