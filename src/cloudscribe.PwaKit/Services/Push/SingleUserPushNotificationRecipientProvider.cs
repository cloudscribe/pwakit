using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class SingleUserPushNotificationRecipientProvider : IPushNotificationRecipientProvider
    {
        public SingleUserPushNotificationRecipientProvider(
            IPushSubscriptionStore pushSubscriptionStore
            )
        {
            _pushSubscriptionStore = pushSubscriptionStore;
        }

        private readonly IPushSubscriptionStore _pushSubscriptionStore;

        public string Name { get; } = "SingleUserPushNotificationRecipientProvider";

        public async Task<IEnumerable<PushDeviceSubscription>> GetRecipients(PushQueueItem pushQueueItem, CancellationToken cancellationToken)
        {
            return await _pushSubscriptionStore.GetSubscriptionsForUser(pushQueueItem.TenantId, pushQueueItem.RecipientProviderCustom1, cancellationToken);

        }

    }
}
