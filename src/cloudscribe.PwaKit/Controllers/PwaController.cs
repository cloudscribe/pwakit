using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Controllers
{
    public class PwaController : Controller
    {
        public PwaController(
            IServiceWorkerBuilder serviceWorkerBuilder,
            IGeneratePwaInitScript serviceWorkerInitScriptGenerator,
            IOptions<PwaOptions> pwaOptionsAccessor,
            IPushSubscriptionStore subscriptionStore, 
            IPushNotificationService notificationService, 
            IPushNotificationsQueue pushNotificationsQueue,
            IUserIdResolver userIdResolver,
            ITenantIdResolver tenantIdResolver
            )
        {
            _serviceWorkerBuilder = serviceWorkerBuilder;
            _serviceWorkerInitScriptGenerator = serviceWorkerInitScriptGenerator;
            _options = pwaOptionsAccessor.Value;

            _subscriptionStore = subscriptionStore;
            _notificationService = notificationService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _userIdResolver = userIdResolver;
            _tenantIdResolver = tenantIdResolver;
        }

        private readonly IServiceWorkerBuilder _serviceWorkerBuilder;
        private readonly IGeneratePwaInitScript _serviceWorkerInitScriptGenerator;
        private readonly PwaOptions _options;

        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IUserIdResolver _userIdResolver;
        private readonly ITenantIdResolver _tenantIdResolver;

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> ServiceWorker()
        {
            Response.ContentType = "application/javascript; charset=utf-8";

            //TODO ?: do we need to cache this,suppose to load at least every 24 hours
            // I think the browser makes head requests to see if any changes
            //Response.Headers[HeaderNames.CacheControl] = $"max-age={_options.ServiceWorkerCacheControlMaxAge}";

            var sw = await _serviceWorkerBuilder.Build(HttpContext);


            return Content(sw);

        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> Init()
        {
            var script = await _serviceWorkerInitScriptGenerator.BuildPwaInitScript(HttpContext, Url);
            if(string.IsNullOrWhiteSpace(script))
            {
                return NotFound();
            }

            Response.ContentType = "application/javascript; charset=utf-8";
            
            return Content(script);

        }

        [HttpGet]
        [HttpHead]
        public IActionResult Offline()
        {

            return View();
        }

        #region Push Notifications

        [Authorize(Policy ="PushNotificationAdminPolicy")]
        [HttpGet]
        public IActionResult TestPush()
        {
            return View();
        }

        [Authorize(Policy = "PushNotificationAdminPolicy")]
        [HttpGet]
        public IActionResult GeneratePushKeys()
        {
            return View();
        }



        [HttpGet]
        public ContentResult GetPublicKey()
        {
            return Content(_notificationService.PublicKey, "text/plain");
        }

        
        [HttpPost]
        public async Task<IActionResult> Subscription([FromBody]Lib.Net.Http.WebPush.PushSubscription subscription)
        {
            if(subscription != null && !string.IsNullOrEmpty(subscription.Endpoint))
            {
                var newSub = new PushDeviceSubscription(subscription);
                newSub.TenantId = _tenantIdResolver.GetTenantId();
                newSub.UserId = _userIdResolver.GetUserId(User);
                newSub.UserAgent = Request.Headers["User-Agent"].ToString();
                newSub.CreatedFromIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                await _subscriptionStore.SaveSubscription(newSub);
            }
            
            return NoContent();
        }

        
        [HttpDelete]
        public async Task<IActionResult> Subscription(string endpoint)
        {
            await _subscriptionStore.DeleteSubscription(endpoint);

            return NoContent();
        }

        [Authorize(Policy = "PushNotificationAdminPolicy")]
        [HttpPost]
        public IActionResult SendNotification([FromBody]PushMessageModel message)
        {
            var pushMessage = new PushMessage(message.Notification)
            {
                Topic = message.Topic,
                Urgency = message.Urgency
            };

            var queueItem = new PushQueueItem(pushMessage, BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);
            queueItem.TenantId = _tenantIdResolver.GetTenantId();

            _pushNotificationsQueue.Enqueue(queueItem);

            return NoContent();
        }



        #endregion



    }
}
