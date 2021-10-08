using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultConfigureWorkboxPreCache : IConfigureWorkboxPreCache
    {
        public DefaultConfigureWorkboxPreCache(
            IEnumerable<IPreCacheItemProvider> preCacheProviders
            )
        {
            _preCacheProviders = preCacheProviders;
        }

        private readonly IEnumerable<IPreCacheItemProvider> _preCacheProviders;

        public async Task AppendToServiceWorkerScript(StringBuilder sw, PwaOptions options, HttpContext context)
        {
            var items = new List<ServiceWorkerCacheItem>();
            foreach (var provider in _preCacheProviders)
            {
                var i = await provider.GetItems();
                items.AddRange(i);
            }

            if (items.Count == 0) { return; }

            //sw.Append("const precacheController = new workbox.precaching.PrecacheController();");
            //sw.Append("precacheController.addToCacheList([");

            var comma = "";

            sw.Append("workbox.precaching.precacheAndRoute([");
            foreach (var item in items)
            {
                sw.Append(comma);
                if (!string.IsNullOrEmpty(item.Revision))
                {
                    sw.Append("{");
                    sw.Append("\"url\": \"" + item.Url + "\",");
                    sw.Append("\"revision\": \"" + item.Revision + "\"");

                    sw.Append("}");
                }
                else
                {
                    // jk - from  https://developers.google.com/web/tools/workbox/modules/workbox-precaching#explanation_of_the_precache_list
                    // we need some revision information but we can neither hard-code it into the json config, nor dynamically infer it here from 
                    // "one of Workbox's build tools".  v5 of Workbox requires an explicit null, therefore.

                    sw.Append("{");
                    sw.Append("\"url\": \"" + item.Url + "\",");
                    sw.Append("\"revision\": \"null\"");

                    sw.Append("}");

                }

                comma = ",";
            }

            sw.Append("]");

            sw.Append(",{"); //begin options

            sw.Append("plugins: [");
            sw.Append("new workbox.broadcastUpdate.BroadcastUpdatePlugin({");
            sw.Append("channelName: 'app-channel'");
            sw.Append("}),");
            sw.Append("]");

            sw.Append("}"); //end options


            sw.Append(");");


            //sw.Append("self.addEventListener('install', (event) => {");
            //sw.Append("console.log('install event for pre-cache controller');");
            //sw.Append("event.waitUntil(precacheController.install());");
            //sw.Append("});");

            //sw.Append("self.addEventListener('activate', (event) => {");
            //sw.Append("console.log('activate event for pre-cache controller');");
            ////sw.Append("event.waitUntil(precacheController.cleanup());");
            //sw.Append("});");

            //sw.Append("self.addEventListener('fetch', (event) => {");
            //sw.Append("console.log('fetch event for pre-cache controller for url ' + event.request.url);");
            //sw.Append("const cacheKey = precacheController.getCacheKeyForURL(event.request.url);");

            //sw.Append("event.waitUntil(");
            //sw.Append("caches.match(cacheKey).then(function(response) {");

            //sw.Append("console.log('response was');");
            //sw.Append("console.log(response);");
            //sw.Append("event.respondWith(response);");
            
            //sw.Append("});");

            //sw.Append(");");//wiatuntil




            //sw.Append("});");//end fetch


        }

    }

    
}
