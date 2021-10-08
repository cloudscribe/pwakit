using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultConfigureServiceWorkerReloading : IConfigureServiceWorkerReloading
    {
        public DefaultConfigureServiceWorkerReloading(
            IWorkboxCacheSuffixProvider workboxCacheSuffixProvider
            )
        {
            _workboxCacheSuffixProvider = workboxCacheSuffixProvider;
        }

        private readonly IWorkboxCacheSuffixProvider _workboxCacheSuffixProvider;


        public async Task AppendToServiceWorkerScript(StringBuilder sw, PwaOptions options, HttpContext context)
        {
            var cacheSuffix = await _workboxCacheSuffixProvider.GetWorkboxCacheSuffix();

            //if (context.User.Identity.IsAuthenticated)
            //{
            //    sw.Append("workbox.core.setCacheNameDetails({");
            //    sw.Append("prefix: 'web-app-auth',");
            //    sw.Append("suffix: '" + cacheSuffix + "',");
            //    sw.Append("precache: 'auth-user-precache',");
            //    sw.Append("runtime: 'auth-user-runtime-cache'");
            //    sw.Append("});");


            //}
            //else
            //{
            //    sw.Append("workbox.core.setCacheNameDetails({");
            //    sw.Append("prefix: 'web-app-anon',");
            //    sw.Append("suffix: '" + cacheSuffix + "',");
            //    sw.Append("precache: 'unauth-user-precache',");
            //    sw.Append("runtime: 'unauth-user-runtime-cache'");
            //    sw.Append("});");
            //}

            //no longer need separate cahce for auth vs anonymous
            sw.Append("workbox.core.setCacheNameDetails({");
            sw.Append("prefix: 'web-app',");
            sw.Append("suffix: '" + cacheSuffix + "',");
            sw.Append("precache: 'precache',");
            sw.Append("runtime: 'runtime-cache'");
            sw.Append("});");


            //https://developers.google.com/web/tools/workbox/reference-docs/latest/workbox.core

            //Force a service worker to become active, instead of waiting. This is normally used in conjunction with clientsClaim()
            sw.Append("self.skipWaiting();");
            sw.Append("workbox.core.clientsClaim();");


            //https://developers.google.com/web/tools/workbox/reference-docs/latest/workbox.core

            //Force a service worker to become active, instead of waiting. This is normally used in conjunction with clientsClaim()
            //sw.Append("workbox.skipWaiting();");
            //sw.Append("workbox.clientsClaim();");

            //https://github.com/GoogleChrome/workbox/issues/1407
            //cleanup old caches
            sw.Append("let currentCacheNames = Object.assign(");
            sw.Append("{ precacheTemp: workbox.core.cacheNames.precache + \"-temp\" },");
            sw.Append("workbox.core.cacheNames");
            sw.Append(");");

            sw.Append("self.addEventListener(\"activate\", function(event) {");

            sw.Append("event.waitUntil(");
            sw.Append("caches.keys().then(function(cacheNames) {");
            sw.Append("let validCacheSet = new Set(Object.values(currentCacheNames));");
            sw.Append("return Promise.all(");
            sw.Append("cacheNames");
            sw.Append(".filter(function(cacheName) {");
            sw.Append("return !validCacheSet.has(cacheName);");
            sw.Append("})");
            sw.Append(".map(function(cacheName) {");
            sw.Append("return caches.delete(cacheName);");
            sw.Append("})");
            sw.Append(");");

            sw.Append("})");
            sw.Append(");");

            if(options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('activate event fired');");
            }

            

            sw.Append("});");

            //new in 4.0.x
            sw.Append("workbox.precaching.cleanupOutdatedCaches();");

        }

    }
}
