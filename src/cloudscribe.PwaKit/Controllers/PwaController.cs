using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Lib.Net.Http.WebPush;
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
            IPushNotificationsQueue pushNotificationsQueue
            )
        {
            _serviceWorkerBuilder = serviceWorkerBuilder;
            _serviceWorkerInitScriptGenerator = serviceWorkerInitScriptGenerator;
            _options = pwaOptionsAccessor.Value;

            _subscriptionStore = subscriptionStore;
            _notificationService = notificationService;
            _pushNotificationsQueue = pushNotificationsQueue;
        }

        private readonly IServiceWorkerBuilder _serviceWorkerBuilder;
        private readonly IGeneratePwaInitScript _serviceWorkerInitScriptGenerator;
        private readonly PwaOptions _options;

        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;

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


        [HttpGet]
        public ContentResult GetPublicKey()
        {
            return Content(_notificationService.PublicKey, "text/plain");
        }

        // POST push-notifications-api/subscriptions
        [HttpPost]
        public async Task<IActionResult> Subscription([FromBody]Lib.Net.Http.WebPush.PushSubscription subscription)
        {
            await _subscriptionStore.StoreSubscriptionAsync(subscription);

            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete]
        public async Task<IActionResult> Subscription(string endpoint)
        {
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);

            return NoContent();
        }

        // POST push-notifications-api/notifications
        [HttpPost]
        public IActionResult SendNotification([FromBody]PushMessageModel message)
        {
            _pushNotificationsQueue.Enqueue(new PushMessage(message.Notification)
            {
                Topic = message.Topic,
                Urgency = message.Urgency
            });

            return NoContent();
        }



        #endregion



    }
}
