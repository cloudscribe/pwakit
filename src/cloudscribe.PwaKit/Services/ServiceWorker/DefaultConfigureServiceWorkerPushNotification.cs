using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultConfigureServiceWorkerPushNotification : IConfigureServiceWorkerPushNotification
    {

        public Task AppendToServiceWorkerScript(StringBuilder sw, PwaOptions options, HttpContext context)
        {



            sw.Append("self.importScripts('/js/push-notifications-controller.js');");



            sw.Append("self.addEventListener('push', function (event) {");

            
            sw.Append("var json = event.data.json();");

            if(options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(json);");
            }

            sw.Append("const precacheCacheName = workbox.core.cacheNames.precache;");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('pre-cache name is ' + precacheCacheName);");
            }


            // --- content add begin

            sw.Append("if(json.messageType === 'newcontent') {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('content add');");
            }

            sw.Append("caches.open(precacheCacheName).then(function(cache) {");

            sw.Append("fetch(json.data).then(function (response) {");
            sw.Append("cache.put(json.data, response.clone()).then(function () {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('added item to cache');");
            }

            sw.Append("});");
            sw.Append("});");


            sw.Append("});"); //end cache open
            sw.Append("}");
            // --- content add end


            // --- content update begin

            sw.Append("if(json.messageType === 'contentupdate') {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('content update');");
            }
               
            sw.Append("caches.open(precacheCacheName).then(function(cache) {");


            sw.Append("var key = workbox.precaching.getCacheKeyForURL(json.data);");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('key is ' + key);");
            }
            
            sw.Append("cache.delete(key).then(function(response) {");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(response);");
            }
                
            sw.Append("if(response === true) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('deleted from cache');");
            }
                

            sw.Append("fetch(key).then(function (response) {");
            sw.Append("cache.put(key, response.clone()).then(function () {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('updated cache');");
            }
                
            sw.Append("});");
            sw.Append("});");

            //sw.Append("var refreshMessage = {");
            //sw.Append("type: 'refresh',");
            //sw.Append("url: json.data");
            //sw.Append("};");
            //sw.Append("var s = JSON.stringify(refreshMessage);");

            //sw.Append("self.clients.matchAll().then(function (clients) {");
            //sw.Append("clients.forEach(function (client) {");
            //sw.Append("client.postMessage(s);");
            //sw.Append("});");
            //sw.Append("});");

            sw.Append("}"); //end if delete response === true
           
            sw.Append("});"); //end delete



            sw.Append("})"); //end cache open

            sw.Append("};"); //end if contentupdate

            /// --- content update end

            /// -- content delete

            sw.Append("if(json.messageType === 'contentdelete') {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('content delete');");
            }

            sw.Append("caches.open(precacheCacheName).then(function(cache) {");

            sw.Append("var key = workbox.precaching.getCacheKeyForURL(json.data);");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('key is ' + key);");
            }

            sw.Append("cache.delete(key).then(function(response) {");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(response);");
            }

            sw.Append("if(response === true) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('removed item from cache');");
            }
            sw.Append("}"); //end if true

            sw.Append("});"); //end delete


            sw.Append("});"); //end cache open
            sw.Append("}");

            /// --- content delete end



            //sw.Append("if (!(self.Notification && self.Notification.permission === 'granted')) {");
            //sw.Append("return;");
            //sw.Append("}");


            sw.Append("if(json.messageType === 'Visible') {");
            sw.Append("event.waitUntil(self.registration.showNotification(json.title, json));");
            sw.Append("} else {");
            sw.Append("console.log('non visible message');");

            //sw.Append("console.log(json.data);");

            //sw.Append("if(contentUpdateHandler) {");
            //sw.Append("event.waitUntil(contentUpdateHandler(json));");
            //sw.Append("}");

            sw.Append("return;"); //cancel notification
            sw.Append("}");

            sw.Append("});"); //end push event




            sw.Append("self.addEventListener('pushsubscriptionchange', function (event) {");
            sw.Append("const handlePushSubscriptionChangePromise = Promise.resolve();");

            sw.Append("if (event.oldSubscription) {");
            sw.Append("handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {");
            sw.Append("return PushNotificationsController.discardPushSubscription(event.oldSubscription);");
            sw.Append("});");
            sw.Append("}");

            sw.Append("if (event.newSubscription) {");
            sw.Append("handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {");
            sw.Append("return PushNotificationsController.storePushSubscription(event.newSubscription);");
            sw.Append("});");
            sw.Append("}");

            sw.Append("if (!event.newSubscription) {");
            sw.Append("handlePushSubscriptionChangePromise = handlePushSubscriptionChangePromise.then(function () {");
            sw.Append("return PushNotificationsController.retrievePublicKey().then(function (applicationServerPublicKey) {");
            sw.Append("return pushServiceWorkerRegistration.pushManager.subscribe({");
            sw.Append("userVisibleOnly: true,");
            sw.Append("applicationServerKey: applicationServerPublicKey");
            sw.Append("}).then(function (pushSubscription) {");
            sw.Append("return PushNotificationsController.storePushSubscription(pushSubscription);");
            sw.Append("});");
            sw.Append("});");
            sw.Append("});");
            sw.Append("}");


            sw.Append("event.waitUntil(handlePushSubscriptionChangePromise);");
            sw.Append("});");

            sw.Append("self.addEventListener('notificationclick', function (event) {");

            sw.Append("console.log(event);");
            //sw.Append("event.notification.close();");
            sw.Append("});");



            return Task.CompletedTask;
        }



    }
}
