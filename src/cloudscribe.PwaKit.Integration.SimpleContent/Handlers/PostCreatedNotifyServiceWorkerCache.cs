using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PostCreatedNotifyServiceWorkerCache : IHandlePostCreated
    {
        public PostCreatedNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue,
            IProjectSettingsResolver projectSettingsResolver,
            IBlogUrlResolver blogUrlResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _projectSettingsResolver = projectSettingsResolver;
            _blogUrlResolver = blogUrlResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IProjectSettingsResolver _projectSettingsResolver;
        private readonly IBlogUrlResolver _blogUrlResolver;

        public async Task Handle(string projectId, IPost post, CancellationToken cancellationToken = default(CancellationToken))
        {
            var project = await _projectSettingsResolver.GetCurrentProjectSettings(cancellationToken);

            var url = await _blogUrlResolver.ResolvePostUrl(post, project);

            var message = new PushMessageModel()
            {
                MessageType = "newcontent",
                Body = "New content",
                Data = url

            };
            
            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);

            queueItem.TenantId = post.BlogId;

            _pushNotificationsQueue.Enqueue(queueItem);



            //TODO: need to extract all image urls and sned message to sw to add to cache if not in there already


        }
    }
}
