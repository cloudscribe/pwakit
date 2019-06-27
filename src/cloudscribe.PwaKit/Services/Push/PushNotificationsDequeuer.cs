using cloudscribe.PwaKit.Interfaces;
using Lib.Net.Http.WebPush;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    internal class PushNotificationsDequeuer : IHostedService
    {
        //private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;
        private readonly IPushSubscriptionStore _pushSubscriptionStore;
        private readonly IPushNotificationsQueue _messagesQueue;
        private readonly IPushNotificationService _notificationService;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();

        private Task _dequeueMessagesTask;

        public PushNotificationsDequeuer(
            IPushNotificationsQueue messagesQueue,
            IPushSubscriptionStore pushSubscriptionStore, 
            IPushNotificationService notificationService
            )
        {
            _pushSubscriptionStore = pushSubscriptionStore;
            _messagesQueue = messagesQueue;
            _notificationService = notificationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                PushMessage message = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {

                    await _pushSubscriptionStore.ForEachSubscriptionAsync((PushSubscription subscription) =>
                    {
                        // Fire-and-forget 
                        _notificationService.SendNotificationAsync(subscription, message, _stopTokenSource.Token);
                    }, _stopTokenSource.Token);

                    //using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    //{
                    //    await subscriptionStoreAccessor.PushSubscriptionStore.ForEachSubscriptionAsync((PushSubscription subscription) =>
                    //    {
                    //        // Fire-and-forget 
                    //        _notificationService.SendNotificationAsync(subscription, message, _stopTokenSource.Token);
                    //    }, _stopTokenSource.Token);
                    //}

                }
            }

        }
    }
}
