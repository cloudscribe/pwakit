using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PageDeleteNotifyServiceWorkerCache : IHandlePagePreDelete
    {
        public PageDeleteNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue,
            IPageQueries pageQueries,
            IProjectSettingsResolver projectSettingsResolver,
            IPageUrlResolver pageUrlResolver,
            IUserIdResolver userIdResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _pageQueries = pageQueries;
            _projectSettingsResolver = projectSettingsResolver;
            _pageUrlResolver = pageUrlResolver;
            _userIdResolver = userIdResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IPageQueries _pageQueries;
        private readonly IProjectSettingsResolver _projectSettingsResolver;
        private readonly IPageUrlResolver _pageUrlResolver;
        private readonly IUserIdResolver _userIdResolver;

        public async Task Handle(string projectId, string pageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var page = await _pageQueries.GetPage(projectId, pageId, cancellationToken);

            var url = await _pageUrlResolver.ResolvePageUrl(page);

            var message = new PushMessageModel()
            {
                MessageType = "contentdelete",
                Body = "Content deleted",
                Data = url

            };

            if (page.Slug == "home")
            {
                message.Data = "/";
            }

            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllButCurrentUserPushNotificationRecipientProvider);

            queueItem.TenantId = page.ProjectId;
            queueItem.RecipientProviderCustom1 = _userIdResolver.GetCurrentUserId();

            _pushNotificationsQueue.Enqueue(queueItem);



        }
    }

}
