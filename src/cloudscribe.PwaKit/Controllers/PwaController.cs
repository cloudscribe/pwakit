using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using cloudscribe.Web.Common.Helpers;
using Lib.Net.Http.WebPush;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
            ITenantIdResolver tenantIdResolver,
            IResourceHelper resourceHelper
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
            _resourceHelper = resourceHelper;
        }

        private readonly IServiceWorkerBuilder _serviceWorkerBuilder;
        private readonly IGeneratePwaInitScript _serviceWorkerInitScriptGenerator;
        private readonly PwaOptions _options;

        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IUserIdResolver _userIdResolver;
        private readonly ITenantIdResolver _tenantIdResolver;
        private readonly IResourceHelper _resourceHelper;

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> ServiceWorker()
        {
            var sw = await _serviceWorkerBuilder.Build(HttpContext);

            if(string.IsNullOrWhiteSpace(sw))
            {
                return NotFound();
            }

            //TODO ?: do we need to cache this,suppose to load at least every 24 hours
            // I think the browser makes head requests to see if any changes
            //Response.Headers[HeaderNames.CacheControl] = $"max-age={_options.ServiceWorkerCacheControlMaxAge}";
            Response.ContentType = "application/javascript; charset=utf-8";

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

        [Authorize]
        [HttpGet]
        public IActionResult NotificationSettings()
        {
            return View();
        }

        [Authorize(Policy ="PushNotificationAdminPolicy")]
        [HttpGet]
        public IActionResult PushConsole()
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
        public IActionResult BroadcastNotification([FromBody]PushMessageModel message)
        {
            var queueItem = new PushQueueItem(
                message, 
                BuiltInRecipientProviderNames.AllSubscribersPushNotificationRecipientProvider);

            queueItem.TenantId = _tenantIdResolver.GetTenantId();

            _pushNotificationsQueue.Enqueue(queueItem);

            return NoContent();
        }

        [Authorize(Policy = "PushNotificationAdminPolicy")]
        [HttpPost]
        public IActionResult SendNotificationToSelf([FromBody]PushMessageModel message)
        {
            
            var queueItem = new PushQueueItem(
                message, 
                BuiltInRecipientProviderNames.SingleUserPushNotificationRecipientProvider);

            queueItem.TenantId = _tenantIdResolver.GetTenantId();
            queueItem.RecipientProviderCustom1 = _userIdResolver.GetUserId(User);

            _pushNotificationsQueue.Enqueue(queueItem);

            return NoContent();
        }



        #endregion

        #region staticResources

        private IActionResult GetResult(string resourceName, string contentType)
        {
            var assembly = typeof(PwaController).GetTypeInfo().Assembly;
            resourceName = _resourceHelper.ResolveResourceIdentifier(resourceName);
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream == null)
            {
                //Log.LogError("resource not found for " + resourceName);
                return NotFound();
            }

            //Log.LogDebug("resource found for " + resourceName);

            var status = ETagGenerator.AddEtagForStream(HttpContext, resourceStream);
            if (status != null) { return status; } //304

            return new FileStreamResult(resourceStream, contentType);
        }

        // /pwa/js/
        [HttpGet]
        [Route("pwa/js/{slug}")]
        [AllowAnonymous]
        public virtual IActionResult Js()
        {
            var baseSegment = "cloudscribe.PwaKit.js.";

            var requestPath = HttpContext.Request.Path.Value;
            //Log.LogDebug(requestPath + " requested");

            if (requestPath.Length < "/pwa/js/".Length) return NotFound();

            var seg = requestPath.Substring("/pwa/js/".Length);
            var ext = Path.GetExtension(requestPath);
            var mimeType = _resourceHelper.GetMimeType(ext);

            return GetResult(baseSegment + seg, mimeType);
        }


        #endregion



    }
}
