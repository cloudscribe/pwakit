using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.SimpleContent.Models;
using cloudscribe.SimpleContent.Models.EventHandlers;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.SimpleContent.Handlers
{
    public class PostDeleteNotifyServiceWorkerCache : IHandlePostPreDelete
    {
        public PostDeleteNotifyServiceWorkerCache(
            IPushNotificationsQueue pushNotificationsQueue,
            IProjectSettingsResolver projectSettingsResolver,
            IPostQueries postQueries,
            IBlogUrlResolver blogUrlResolver
            )
        {
            _pushNotificationsQueue = pushNotificationsQueue;
            _projectSettingsResolver = projectSettingsResolver;
            _postQueries = postQueries;
            _blogUrlResolver = blogUrlResolver;
        }

        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IProjectSettingsResolver _projectSettingsResolver;
        private readonly IPostQueries _postQueries;
        private readonly IBlogUrlResolver _blogUrlResolver;

        public async Task Handle(string projectId, string postId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var post = await _postQueries.GetPost(projectId, postId, cancellationToken);

            var project = await _projectSettingsResolver.GetCurrentProjectSettings(cancellationToken);

            var url = await _blogUrlResolver.ResolvePostUrl(post, project);

            var message = new PushMessageModel()
            {
                MessageType = "contentdelete",
                Body = "Content deleted",
                Data = url

            };
            
            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);

            queueItem.TenantId = post.BlogId;

            _pushNotificationsQueue.Enqueue(queueItem);

        }
    }
}
