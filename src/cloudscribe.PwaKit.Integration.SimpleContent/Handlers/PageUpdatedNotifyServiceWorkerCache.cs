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
            IPushNotificationsQueue pushNotificationsQueue,
            IPageUrlResolver pageUrlResolver,
            IUserIdResolver userIdResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _pageUrlResolver = pageUrlResolver;
            _userIdResolver = userIdResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IPageUrlResolver _pageUrlResolver;
        private readonly IUserIdResolver _userIdResolver;

        public async Task Handle(
            string projectId,
            IPage page,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var url = await _pageUrlResolver.ResolvePageUrl(page);

            var message = new PushMessageModel()
            {
                MessageType = "contentupdate",
                Body = "Content updated",
                Data = url

            };

            if(page.Slug == "home")
            {
                message.Data = "/";
            }

            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllButCurrentUserPushNotificationRecipientProvider);

            queueItem.TenantId = page.ProjectId;
            queueItem.RecipientProviderCustom1 = _userIdResolver.GetCurrentUserId();

            _pushNotificationsQueue.Enqueue(queueItem);

            //TODO: need to extract all image urls and sned message to sw to add to cache if not in there already
            // or maybe need an event for file system when files added or deleted

            

        }

    }
}
