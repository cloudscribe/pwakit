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

            #region ClientMessageBus

            sw.Append("var idb;");
            sw.Append("if(self.indexedDB) { ");
            sw.Append("idb = self.indexedDB; ");
            if(options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('IndexedDB is supported');");
            }
            sw.Append("} ");



            sw.Append("function sendMessage(msg) {");

            sw.Append("if ('BroadcastChannel' in self) {");

            sw.Append("const channel = new BroadcastChannel('app-channel');");
            sw.Append("channel.postMessage(msg);");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('sent message to client');");
            }
               
            sw.Append("} else {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('message not sent to client because BroadcastChannel not supported by the browser');");
            } 
            sw.Append("} "); //endif broadcast channel
            sw.Append("} "); //end send message


            sw.Append("var messageListBuilder = function() {");
            sw.Append("var priv;");

            sw.Append("function build() {");

            sw.Append("if(priv) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('returning ready function');");
            }
                
            sw.Append("return;");

            sw.Append("} else {");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('building messagelist function');");
            }
              
            sw.Append("priv = {");

            sw.Append("addMessage : function(msg) {");
            
            sw.Append("if(idb) {");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('add message');");
            }
              
            sw.Append("var request = idb.open('sw_DB', 1);");

            sw.Append("request.onsuccess = function(event) {");

            sw.Append("var db = event.target.result;");
            sw.Append("var transaction = db.transaction('clientmessages', 'readwrite');");

            sw.Append("transaction.onsuccess = function(event) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('IndexDB transaction success');");
            }
              
            sw.Append("}; ");//endtransaction.success

            sw.Append("var msgStore = transaction.objectStore('clientmessages');");
            sw.Append("var db_op_req = msgStore.add(msg);");

            sw.Append("db_op_req.onsuccess = function(event) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(event);");
            }
                
            sw.Append("}; "); //end db_op_req onsucess

            sw.Append("}; "); //end request onsuccess

            sw.Append("request.onupgradeneeded = function(event) {");
            sw.Append("var db = event.target.result;");
            sw.Append("var store = db.createObjectStore('clientmessages', {keyPath:'id'});");
            sw.Append("}; ");// end onupgraded

            sw.Append("} else {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('failed to add message because IndexedDB not suppported in this browser');");
            }  
            sw.Append("} ");

            sw.Append("},"); //end add message

            sw.Append("iterate: function(f) {");

            sw.Append("if(idb) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('iterate message');");
            }   
            sw.Append("var request = idb.open('sw_DB', 1);");

            sw.Append("request.onupgradeneeded = function(event) {");
            sw.Append("var db = event.target.result;");
            sw.Append("var store = db.createObjectStore('clientmessages', {keyPath : 'id'});");
            sw.Append("}; ");// end onupgraded


            sw.Append("request.onsuccess = function(event) {");

            sw.Append("var db = event.target.result; ");

            sw.Append("var transaction = db.transaction('clientmessages', 'readwrite');");
            sw.Append("transaction.onsuccess = function(event) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('IndexDb transaction success');");
            }
                
            sw.Append("}; ");//endtransaction.success

            sw.Append("var msgStore = transaction.objectStore('clientmessages');");

            //get all messages and send to client
            sw.Append("msgStore.getAll().onsuccess = function(event) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('get all messages');");
                sw.Append("console.log(event.target.result);");
            }
               
            sw.Append("var list = event.target.result;");

            sw.Append("list.forEach(function(msg) {");
            sw.Append("f(msg);");
            sw.Append("});");//end foreach

            sw.Append("msgStore.clear().onsuccess = function(event) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('clear all messages');");
                sw.Append("console.log(event);");
            }
                
            sw.Append("}; "); //end clear

            sw.Append("}; "); //end getall
           
            sw.Append("}; "); //end request onsuccess

            sw.Append("} else {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('failed to send message because IndexedDB not suppported in this browser');");
            }
                
            sw.Append("} ");



            sw.Append("}"); //end iterate
            sw.Append("};");// end  priv 
            sw.Append("} ");//end if priv
            sw.Append("} ");//end build function

            sw.Append("if(priv) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('returning ready priv');");
            }
               
            sw.Append("return priv;");
            sw.Append("}");

            sw.Append("build();");
            sw.Append("return priv;");
            sw.Append("}; "); //end function

            sw.Append("if(!self.messageList) {");

            sw.Append("self.messageList = messageListBuilder(); ");
            sw.Append("} ");

            
            
            sw.Append("self.addEventListener('message', function(event){");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('SW Received Message ' + new Date().toString());");
                sw.Append("console.log(event);");
            }
                

            sw.Append("if(event.data.type === 'page-ready') {");
            sw.Append("if(self.messageList) {");
            sw.Append("self.messageList.iterate(sendMessage);");

            //sw.Append("sendMessage({type:'hello' });");
            sw.Append("} ");


            sw.Append("}"); //endif windowready
           
            sw.Append("}); ");

            #endregion


            #region pushmanager

            sw.Append("if ('PushManager' in self) {");



            sw.AppendLine("self.importScripts('/pwa/js/push-notifications-controller.js');");
            
            sw.Append("self.addEventListener('push', function (event) {");

            
            sw.Append("var json = event.data.json();");

            if(options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(json);");
            }

            sw.Append("const precacheCacheName = workbox.core.cacheNames.runtime;");
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


            //sw.Append("var key = workbox.precaching.getCacheKeyForURL(json.data);");
            //if (options.EnableServiceWorkerConsoleLog)
            //{
            //    sw.Append("console.log('key is ' + key);");
            //}
            
            sw.Append("cache.delete(json.data).then(function(response) {");

            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(response);");
            }
                
            sw.Append("if(response === true) {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('deleted from cache');");
            }
                

            sw.Append("fetch(json.data).then(function (response) {");
            sw.Append("cache.put(json.data, response.clone()).then(function () {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('updated cache');");
            }

            sw.Append("let msg = { type:'cacheupdate', url: json.data, id: json.data }; ");

            sw.Append("if(self.messageList) {");
            sw.Append("self.messageList.addMessage(msg);");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('added new message ' + new Date().toString())");
            }
                
            
            sw.Append("} ");

          
                
            sw.Append("});");
            sw.Append("});");

           

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
            
            sw.Append("cache.delete(json.data).then(function(response) {");
            
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('removed item from cache');");
            }
            
            sw.Append("});"); //end delete
            
            sw.Append("});"); //end cache open
            sw.Append("}");

            /// --- content delete end




            sw.Append("if(json.messageType === 'Visible') {");
            sw.Append("event.waitUntil(self.registration.showNotification(json.title, json));");
            sw.Append("} else {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('non visible message');");
            }
                
            
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
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log(event);");
            }
                
            //sw.Append("event.notification.close();");
            sw.Append("});");

            sw.Append("} else {");
            if (options.EnableServiceWorkerConsoleLog)
            {
                sw.Append("console.log('PushManager NOT supported');");
            }
                
            sw.Append("} ");

            #endregion

            return Task.CompletedTask;
        }



    }
}
