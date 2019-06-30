using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PageUpdatedNotifyServiceWorkerCache : IHandlePageUpdated
    {
        public PageUpdatedNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;

        public Task Handle(
            string projectId,
            IPage page,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var message = new PushMessageModel()
            {
                MessageType = "contentupdate",
                Body = "/" + page.Slug

            };

            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);

            queueItem.TenantId = page.ProjectId;

            _pushNotificationsQueue.Enqueue(queueItem);


            return Task.CompletedTask;
        }

    }
}
