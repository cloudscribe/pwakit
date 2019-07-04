using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultGeneratePwaInitScript : IGeneratePwaInitScript
    {
        public DefaultGeneratePwaInitScript(
            IOptions<PwaOptions> pwaOptionsAccessor,
            IEnumerable<IRuntimeCacheItemProvider> preCacheProviders,
            IPwaRouteNameProvider pwaRouteNameProvider
            )
        {
            _options = pwaOptionsAccessor.Value;
            _pwaRouteNameProvider = pwaRouteNameProvider;
            _preCacheProviders = preCacheProviders;
        }

        private readonly PwaOptions _options;
        private readonly IPwaRouteNameProvider _pwaRouteNameProvider;
        private readonly IEnumerable<IRuntimeCacheItemProvider> _preCacheProviders;

        public async Task<string> BuildPwaInitScript(HttpContext context, IUrlHelper urlHelper)
        {

            var url = urlHelper.RouteUrl(_pwaRouteNameProvider.GetServiceWorkerRouteName());

            var script = new StringBuilder();

            if (_options.SetupInstallPrompt)
            {
                script.Append("let deferredPrompt;");
                script.Append("let divPrompt = document.getElementById('divPwaInstallPrompt');");
                script.Append("let btnInstall = document.getElementById('btnPwaInstall');");
                script.Append("if(divPrompt && btnInstall) {");


                script.Append("window.addEventListener('beforeinstallprompt', (e) => {");
                // Prevent Chrome 67 and earlier from automatically showing the prompt
                script.Append("e.preventDefault();");
                // Stash the event so it can be triggered later.
                script.Append("deferredPrompt = e;");

                //update the UI notify the user
                script.Append("divPrompt.classList.add('show');");
                script.Append("divPrompt.style.display ='block';");

                script.Append("});");


                script.Append("btnInstall.addEventListener('click', (e) => {");

                script.Append("document.getElementById('divPwaInstallPrompt').classList.remove('show');");
                script.Append("divPrompt.style.display ='none';");
                // Show the prompt
                script.Append("deferredPrompt.prompt();");
                // Wait for the user to respond to the prompt
                script.Append("deferredPrompt.userChoice");
                script.Append(".then((choiceResult) => {");
                script.Append("if (choiceResult.outcome === 'accepted') {");
                script.Append("console.log('User accepted the A2HS prompt');");
                script.Append("} else {");
                script.Append("console.log('User dismissed the A2HS prompt');");
                script.Append("}");
                script.Append("deferredPrompt = null;");
                script.Append("});");
                script.Append("});");


                script.Append("} ");


            }

           

            script.AppendLine("import {Workbox} from '" +_options.WorkBoxWindowModuleUrl + "';");


            script.Append("if ('serviceWorker' in navigator) {");
            //script.Append("window.addEventListener('load', () => {");
            
            var scope = _pwaRouteNameProvider.GetServiceWorkerScope();
            
            script.Append("const wb = new Workbox('" + url + "',{scope: '" + scope + "'});");

            //activated event
            script.Append("wb.addEventListener('activated', (event) => {");

            script.Append("if (!event.isUpdate) {");
            script.Append("console.log('Service worker activated for the first time');");
            script.Append("} else {");
            script.Append("console.log('Service worker activated');");
            script.Append("}");

            var items = new List<ServiceWorkerCacheItem>();
            foreach (var provider in _preCacheProviders)
            {
                var i = await provider.GetItems();
                items.AddRange(i);
            }

            script.Append("const urlsToCache = [");
            var comma = "";

            foreach (var item in items)
            {
                script.Append(comma);
                if (!string.IsNullOrEmpty(item.Revision))
                {
                    script.Append("{");
                    script.Append("\"url\": \"" + item.Url + "\",");
                    script.Append("\"revision\": \"" + item.Revision + "\"");

                    script.Append("}");
                }
                else
                {
                    script.Append("'" + item.Url + "'");
                }

                comma = ",";
            }


            script.Append("];");
            script.Append("wb.messageSW({");
            script.Append("type: 'CACHE_URLS',");
            script.Append("payload: {urlsToCache} ");
            script.Append("});");


           
            
            

            script.Append("});"); //end activated event



            script.Append("wb.addEventListener('waiting', (event) => {");
            script.Append("console.log('new service worker waiting');");
            script.Append("});");
            
            script.Append("wb.addEventListener('installed', (event) => {");
            script.Append("if (!event.isUpdate) {");
            // First-installed code goes here...
            script.Append("console.log('Service worker installed for the first time');");
            script.Append("} else {");
            script.Append("console.log('new service worker installed');");
            script.Append("}");
            script.Append("});");

            script.Append("wb.addEventListener('controlling', (event) => {");
            script.Append("console.log('new service worker controlling');");

            if (_options.ReloadPageOnServiceWorkerUpdate)
            {
                script.Append("var refreshing;");

                if (_options.EnableServiceWorkerConsoleLog)
                {
                    script.Append("console.log('Controller loaded');");
                }

                script.Append("if (refreshing) return;");
                script.Append("refreshing = true;");
                script.Append("if(!window.location.href.indexOf('account') > -1) {");


                if (_options.EnableServiceWorkerConsoleLog)
                {
                    script.Append("console.log('reloading page because service worker updated');");
                }
                //this causes login to fail
                script.Append("window.location.reload();");

                script.Append("}");

            }

            script.Append("});");

            script.Append("wb.addEventListener('redundant', (event) => {");
            script.Append("console.log('service worker redundant fired');");
            script.Append("});");

            script.Append("wb.addEventListener('externalinstalled', (event) => {");
            script.Append("console.log('service worker externalinstalled fired');");
            script.Append("});");

            script.Append("wb.addEventListener('externalwaiting', (event) => {");
            script.Append("console.log('service worker externalwaiting fired');");
            script.Append("});");

            script.Append("wb.addEventListener('externalactivated', (event) => {");
            script.Append("console.log('service worker externalactivated fired');");
            script.Append("});");

            //listen for messages from sw
            script.Append("wb.addEventListener('message', (event) => {");

            script.Append("console.log(`message received from serviceworker`);");
            script.Append("console.log(event);");

            //script.Append("if (event.data.type === 'CACHE_UPDATED') {");
            //script.Append("const {updatedURL} = event.data.payload;");
            //script.Append("console.log(`A newer version of ${updatedURL} is available!`);");
            //script.Append("} else {");
            //script.Append("console.log(`message received from serviceworker`);");
            //script.Append("console.log(event);");
            //script.Append("}");


            script.Append("}); ");// end message listener


            script.Append("if ('BroadcastChannel' in window) {");

            script.Append("const channel = new BroadcastChannel('app-channel');");
            script.Append("channel.onmessage = function(e) {");
            script.Append("console.log('received message on app channel');");
            script.Append("console.log(e);");

            script.Append("if(e.data.type === 'cacheupdate' && e.data.url === window.location.href) {");
            script.Append("console.log('cache update for current url so reloading');");
            script.Append("window.location.reload();");
            script.Append("}");
            script.Append("};");

            script.Append("} else {");
            script.Append("console.log('Broadcast channel not supported');");
            script.Append("} ");

            
            // Register the service worker after event listeners have been added.
            script.Append("wb.register();");


            // script.Append("});"); //end load event

            script.Append("if ('MessageChannel' in window) {");

            script.Append("navigator.serviceWorker.ready.then(function (serviceWorkerRegistration) {");
            script.Append("console.log('serviceworker ready');");
            
            script.Append("setTimeout(function() {");

            script.Append("const messageChannel = new MessageChannel();");
            script.Append("navigator.serviceWorker.controller.postMessage(");
            script.Append("{type:'page-ready'}");
            script.Append(", [messageChannel.port2]);");

            script.Append("},3000);");

            
            script.Append("});"); //end serviceworker ready

            script.Append("} else {");
            script.Append("console.log('MessageChannel not supported');");
            script.Append("} "); //end if MessageChannel



            script.Append("}"); //end serviceworker in navigator


            return script.ToString();
            //return Task.FromResult(script.ToString());


        }



    }
}
