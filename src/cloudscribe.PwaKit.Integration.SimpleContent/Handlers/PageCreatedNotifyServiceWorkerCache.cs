using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PageCreatedNotifyServiceWorkerCache : IHandlePageCreated
    {
        public PageCreatedNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue,
            IPageUrlResolver pageUrlResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _pageUrlResolver = pageUrlResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IPageUrlResolver _pageUrlResolver;

        public async Task Handle(string projectId, IPage page, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = await _pageUrlResolver.ResolvePageUrl(page);

            var message = new PushMessageModel()
            {
                MessageType = "newcontent",
                Body = "New content",
                Data = url

            };

            if (page.Slug == "home")
            {
                message.Data = "/";
            }

            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);

            queueItem.TenantId = page.ProjectId;

            _pushNotificationsQueue.Enqueue(queueItem);

            //TODO: need to extract all image urls and sned message to sw to add to cache if not in there already
            // or maybe need an event for file system when files added or deleted


        }
    }
}
