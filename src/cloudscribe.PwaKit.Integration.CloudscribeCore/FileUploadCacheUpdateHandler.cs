using cloudscribe.FileManager.Web.Events;
using cloudscribe.FileManager.Web.Models;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class FileUploadCacheUpdateHandler : IHandleFilesUploaded
    {
        public FileUploadCacheUpdateHandler(
            IOptions<PwaContentFilesPreCacheOptions> optionsAccessor,
            IPushNotificationsQueue pushNotificationsQueue,
            IUserIdResolver userIdResolver,
            ITenantIdResolver tenantIdResolver
            )
        {
            _options = optionsAccessor.Value;
            _pushNotificationsQueue = pushNotificationsQueue;
            _userIdResolver = userIdResolver;
            _tenantIdResolver = tenantIdResolver;
        }

        private readonly PwaContentFilesPreCacheOptions _options;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IUserIdResolver _userIdResolver;
        private readonly ITenantIdResolver _tenantIdResolver;

        public Task Handle(IEnumerable<UploadResult> uploadInfoList)
        {
            //TODO: would be better to send the whole list in one message
            // but this will require different handling in serviceworker

            foreach(var u in uploadInfoList)
            {
                if(!string.IsNullOrEmpty(u.OriginalUrl))
                {
                    var ext = Path.GetExtension(u.OriginalUrl).ToLower(); 
                    if(_options.FileExtensionsToCache.Contains(ext))
                    {
                        SendNewContentNotification(u.OriginalUrl);
                    }
                }

                if (!string.IsNullOrEmpty(u.ResizedUrl))
                {
                    var ext = Path.GetExtension(u.ResizedUrl).ToLower();
                    if (_options.FileExtensionsToCache.Contains(ext))
                    {
                        SendNewContentNotification(u.ResizedUrl);
                    }
                }

                if (!string.IsNullOrEmpty(u.ThumbUrl))
                {
                    var ext = Path.GetExtension(u.ThumbUrl).ToLower();
                    if (_options.FileExtensionsToCache.Contains(ext))
                    {
                        SendNewContentNotification(u.ThumbUrl);
                    }
                }
            }



            return Task.CompletedTask;
        }

        private void SendNewContentNotification(string url)
        {
            var message = new PushMessageModel()
            {
                MessageType = "newcontent",
                Body = "New content",
                Data = url

            };
            
            var queueItem = new PushQueueItem(
                message,
                BuiltInRecipientProviderNames.AllButCurrentUserPushNotificationRecipientProvider);

            queueItem.TenantId = _tenantIdResolver.GetTenantId();
            queueItem.RecipientProviderCustom1 = _userIdResolver.GetCurrentUserId();

            _pushNotificationsQueue.Enqueue(queueItem);
        }

    }
}
