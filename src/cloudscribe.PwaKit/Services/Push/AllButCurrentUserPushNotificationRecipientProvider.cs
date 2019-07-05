using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class AllButCurrentUserPushNotificationRecipientProvider : IPushNotificationRecipientProvider
    {
        public AllButCurrentUserPushNotificationRecipientProvider(IPushSubscriptionStore pushSubscriptionStore)
        {
            _pushSubscriptionStore = pushSubscriptionStore;
        }

        private readonly IPushSubscriptionStore _pushSubscriptionStore;

        public string Name { get; } = "AllButCurrentUserPushNotificationRecipientProvider";

        public async Task<IEnumerable<PushDeviceSubscription>> GetRecipients(PushQueueItem pushQueueItem, CancellationToken cancellationToken)
        {
            return await _pushSubscriptionStore.GetAllSubscriptionsExceptForUser(pushQueueItem.TenantId, pushQueueItem.RecipientProviderCustom1, cancellationToken);

        }

    }
}
