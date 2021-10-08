using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class ServiceWorkerBuilder : IServiceWorkerBuilder
    {
        public ServiceWorkerBuilder(
            IOptions<PwaOptions> pwaOptionsAccessor,
            IConfigureServiceWorkerReloading configureServiceWorkerReloading,
            IConfigureWorkboxPreCache configureWorkboxPreCache,
            IConfigureWorkboxNetworkOnlyRoutes configureWorkboxNetworkOnlyRoutes,
            IConfigureWorkboxCacheFirstRoutes configureWorkboxCacheFirstRoutes,
            IConfigureWorkboxNetworkFirstRoutes configureWorkboxNetworkFirstRoutes,
            IConfigureWorkboxOfflineGoogleAnalytics configureGoogleAnalytics,
            IConfigureWorkboxCatchHandler configureWorkboxCatchHandler,
            IConfigureServiceWorkerPushNotification configureServiceWorkerPushNotification,
            IEnumerable<IAddCodeToServiceWorker> addCodeToServiceWorkers
            )
        {
            _options = pwaOptionsAccessor.Value;
            _configureServiceWorkerReloading = configureServiceWorkerReloading;
            _configureWorkboxPreCache = configureWorkboxPreCache;
            _configureWorkboxNetworkOnlyRoutes = configureWorkboxNetworkOnlyRoutes;
            _configureWorkboxCacheFirstRoutes = configureWorkboxCacheFirstRoutes;
            _configureWorkboxNetworkFirstRoutes = configureWorkboxNetworkFirstRoutes;
            _configureGoogleAnalytics = configureGoogleAnalytics;
            _configureWorkboxCatchHandler = configureWorkboxCatchHandler;
            _configureServiceWorkerPushNotification = configureServiceWorkerPushNotification;
            _addCodeToServiceWorkers = addCodeToServiceWorkers;
        }

        private readonly PwaOptions _options;
        private readonly IConfigureServiceWorkerReloading _configureServiceWorkerReloading;
        private readonly IConfigureWorkboxPreCache _configureWorkboxPreCache;
        private readonly IConfigureWorkboxNetworkOnlyRoutes _configureWorkboxNetworkOnlyRoutes;
        private readonly IConfigureWorkboxCacheFirstRoutes _configureWorkboxCacheFirstRoutes;
        private readonly IConfigureWorkboxNetworkFirstRoutes _configureWorkboxNetworkFirstRoutes;
        private readonly IConfigureWorkboxOfflineGoogleAnalytics _configureGoogleAnalytics;
        private readonly IConfigureWorkboxCatchHandler _configureWorkboxCatchHandler;
        private readonly IConfigureServiceWorkerPushNotification _configureServiceWorkerPushNotification;
        private readonly IEnumerable<IAddCodeToServiceWorker> _addCodeToServiceWorkers;



        public async Task<string> Build(HttpContext context, IUrlHelper urlHelper)
        {
            var sw = new StringBuilder();

            if (_options.RequireCookieConsentBeforeRegisteringServiceWorker)
            {
                var consentFeature = context.Features.Get<ITrackingConsentFeature>();
                if(consentFeature != null)
                {
                    if(!consentFeature.CanTrack)
                    {

                        sw.Append("importScripts('" + _options.WorkBoxUrl + "');");

                        sw.Append("if (workbox) {");

                        if (_options.EnableServiceWorkerConsoleLog)
                        {
                            sw.Append("console.log(`Workbox is loaded with empty service worker because no cookie consent`);");
                            sw.Append("workbox.setConfig({ debug: true });");
                            //sw.Append("workbox.core.setLogLevel(workbox.core.LOG_LEVELS.debug);");

                        }

                        //force reload of sw and clear cache

                        sw.Append("workbox.core.setCacheNameDetails({");
                        sw.Append("prefix: 'web-app-no-sw',");
                        sw.Append("suffix: 'no-cookieconsent',");
                        sw.Append("precache: 'no-precache',");
                        sw.Append("runtime: 'not-used-runtime-cache'");
                        sw.Append("});");


                        //Force a service worker to become active, instead of waiting. This is normally used in conjunction with clientsClaim()
                        sw.Append("self.skipWaiting();");
                        sw.Append("workbox.core.clientsClaim();");

                        sw.Append("} else {");

                        if (_options.EnableServiceWorkerConsoleLog)
                        {
                            sw.Append("console.log(`Workbox didn't load`);");
                        }

                        sw.Append("}");


                        return sw.ToString();
                    }
                }
            }



            sw.Append("importScripts('" + _options.WorkBoxUrl + "');");

            sw.Append("if (workbox) {");

            if(_options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(`Workbox is loaded`);");
                sw.Append("workbox.setConfig({ debug: true });");
               //sw.Append("workbox.core.setLogLevel(workbox.core.LOG_LEVELS.debug);");

            }
            
            await _configureServiceWorkerReloading.AppendToServiceWorkerScript(sw, _options, context);

            await _configureServiceWorkerPushNotification.AppendToServiceWorkerScript(sw, _options, context, urlHelper);


            //The order of the Route registration is important. If multiple Routes are registered that could handle a request, the Route that is registered first will be used to respond to the request.
            
            await _configureWorkboxNetworkOnlyRoutes.AppendToServiceWorkerScript(sw, _options, context);

            await _configureWorkboxPreCache.AppendToServiceWorkerScript(sw, _options, context);

            await _configureWorkboxCacheFirstRoutes.AppendToServiceWorkerScript(sw, _options, context);

            await _configureWorkboxNetworkFirstRoutes.AppendToServiceWorkerScript(sw, _options, context);
            
            await _configureGoogleAnalytics.AppendToServiceWorkerScript(sw, _options, context);
            
            foreach (var p in _addCodeToServiceWorkers)
            {
                await p.AppendToServiceWorkerScript(sw, _options, context);
            }
            

            await _configureWorkboxCatchHandler.AppendToServiceWorkerScript(sw, _options, context);

            sw.Append("} else {");

            if (_options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(`Workbox didn't load`);");
            }
             
            sw.Append("}");
            

            return sw.ToString();
        }
        

    }
}
