using cloudscribe.PwaKit.Interfaces;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    internal class PushServicePushNotificationService : IPushNotificationService
    {
        private readonly PushServiceClient _pushClient;
        private readonly IPushSubscriptionStore _pushSubscriptionStore;
        private readonly ILogger _logger;

        public string PublicKey { get { return _pushClient?.DefaultAuthentication?.PublicKey; } }

        public PushServicePushNotificationService(
            PushServiceClient pushClient,
            IPushSubscriptionStore pushSubscriptionStore,
            ILogger<PushServicePushNotificationService> logger
            )
        {
            _pushClient = pushClient;
            _pushSubscriptionStore = pushSubscriptionStore;
            _logger = logger;
        }

        public Task SendNotificationAsync(PushSubscription subscription, PushMessage message)
        {
            return SendNotificationAsync(subscription, message, CancellationToken.None);
        }

        public async Task SendNotificationAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await _pushClient.RequestPushMessageDeliveryAsync(subscription, message, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandlePushMessageDeliveryExceptionAsync(ex, subscription);
            }
        }

        private async Task HandlePushMessageDeliveryExceptionAsync(Exception exception, PushSubscription subscription)
        {
            PushServiceClientException pushServiceClientException = exception as PushServiceClientException;

            if (pushServiceClientException is null)
            {
                _logger?.LogError(exception, "Failed requesting push message delivery to {0}.", subscription.Endpoint);
            }
            else
            {
                if ((pushServiceClientException.StatusCode == HttpStatusCode.NotFound) || (pushServiceClientException.StatusCode == HttpStatusCode.Gone))
                {

                    await _pushSubscriptionStore.DeleteSubscription(subscription.Endpoint);
                    
                    _logger?.LogInformation("Subscription has expired or is no longer valid and has been removed.");
                }
            }
        }
    }
}
