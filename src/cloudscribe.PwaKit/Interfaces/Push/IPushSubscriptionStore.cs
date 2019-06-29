using Lib.Net.Http.WebPush;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IPushSubscriptionStore
    {
        Task StoreSubscriptionAsync(cloudscribe.PwaKit.Models.PushDeviceSubscription subscription);

        Task DiscardSubscriptionAsync(string endpoint);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken);

    }
}
