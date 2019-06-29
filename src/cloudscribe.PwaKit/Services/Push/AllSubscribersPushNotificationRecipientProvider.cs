using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class AllSubscribersPushNotificationRecipientProvider : IPushNotificationRecipientProvider
    {
        public AllSubscribersPushNotificationRecipientProvider(
            IPushSubscriptionStore pushSubscriptionStore
            )
        {
            _pushSubscriptionStore = pushSubscriptionStore;
        }

        private readonly IPushSubscriptionStore _pushSubscriptionStore;

        public string Name { get; } = "AllSubscribersPushNotificationRecipientProvider";

        public async Task<IEnumerable<PushDeviceSubscription>> GetRecipients(PushQueueItem pushQueueItem, CancellationToken cancellationToken)
        {
            return await _pushSubscriptionStore.GetAllSubscriptions(pushQueueItem.TenantId, cancellationToken);

        }

    }
}
