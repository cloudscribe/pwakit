using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PostUpdatedNotifyServiceWorkerCache : IHandlePostUpdated
    {
        public PostUpdatedNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue,
            IProjectSettingsResolver projectSettingsResolver,
            IBlogUrlResolver blogUrlResolver,
            IUserIdResolver userIdResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _projectSettingsResolver = projectSettingsResolver;
            _blogUrlResolver = blogUrlResolver;
            _userIdResolver = userIdResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IProjectSettingsResolver _projectSettingsResolver;
        private readonly IBlogUrlResolver _blogUrlResolver;
        private readonly IUserIdResolver _userIdResolver;

        public async Task Handle(string projectId, IPost post, CancellationToken cancellationToken = default(CancellationToken))
        {

            var project = await _projectSettingsResolver.GetCurrentProjectSettings(cancellationToken);

            var url = await _blogUrlResolver.ResolvePostUrl(post, project);

            var message = new PushMessageModel()
            {
                MessageType = "contentupdate",
                Body = "Content updated",
                Data = url

            };

            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllButCurrentUserPushNotificationRecipientProvider);

            queueItem.TenantId = post.BlogId;
            queueItem.RecipientProviderCustom1 = _userIdResolver.GetCurrentUserId();

            _pushNotificationsQueue.Enqueue(queueItem);


            //TODO: need to extract all image urls and sned message to sw to add to cache if not in there already
            // or maybe need an event for file system when files added or deleted


        }
    }
}
