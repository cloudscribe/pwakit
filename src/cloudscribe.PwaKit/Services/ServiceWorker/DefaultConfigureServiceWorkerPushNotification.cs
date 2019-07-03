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

            //helper functions
            //clients in this case are open browser tabs
            //sw.Append("function send_message_to_client(client, msg){");
            //sw.Append("return new Promise(function(resolve, reject){");
            //sw.Append("var msg_chan = new MessageChannel();");
            //sw.Append("msg_chan.port1.onmessage = function(event){");
            //sw.Append("if(event.data.error){");
            //sw.Append("reject(event.data.error);");
            //sw.Append("} else {");
            //sw.Append("resolve(event.data);");
            //sw.Append("}");
            //sw.Append("};");
            //sw.Append("client.postMessage(\"SW Says: '\"+msg+\"'\", [msg_chan.port2]);");
            //sw.Append("});");
            //sw.Append("} ");

            //sw.Append("function send_message_to_all_clients(msg){");
            //sw.Append("clients.matchAll().then(clients => {");
            //sw.Append("clients.forEach(client => {");
            //sw.Append("send_message_to_client(client, msg).then(m => console.log(\"SW Received Message: \"+m));");
            //sw.Append("})");
            //sw.Append("})");
            //sw.Append("} ");

            ///try indexeddb
            sw.Append("if(self.indexedDB) { console.log('IndexedDB is supported'); } ");
            sw.Append("let idb = self.indexedDB; ");



            sw.Append("function sendMessage(msg) {");
            sw.Append("const channel = new BroadcastChannel('app-channel');");
            sw.Append("channel.postMessage(msg);");
            sw.Append("console.log('sent message to client');");
            sw.Append("} ");


            sw.Append("var messageListBuilder = function() {");
            sw.Append("var priv;");

            sw.Append("function build() {");

            sw.Append("if(priv) {");
            sw.Append("console.log('returning ready function');");
            sw.Append("return;");

            sw.Append("} else {");

            sw.Append("console.log('building function');");
            //sw.Append("var messageBuffer = [];");
            sw.Append("priv = {");

            sw.Append("addMessage : function(msg) {");
            //sw.Append("messageBuffer.push(msg);");
            sw.Append("console.log('add message');");
            //sw.Append("console.log(messageBuffer);");
            
            sw.Append("var request = idb.open('sw_DB', 1);");

            sw.Append("request.onsuccess = function(event) {");

            sw.Append("var db = event.target.result;");
            sw.Append("var transaction = db.transaction('clientmessages', 'readwrite');");

            sw.Append("transaction.onsuccess = function(event) {");
            sw.Append("console.log('[Transaction] ALL DONE!');");
            sw.Append("}; ");//endtransaction.success

            sw.Append("var msgStore = transaction.objectStore('clientmessages');");
            sw.Append("var db_op_req = msgStore.add(msg);");

            sw.Append("db_op_req.onsuccess = function(event) {");
            sw.Append("console.log(event);");
            sw.Append("}; "); //end db_op_req onsucess

            sw.Append("}; "); //end request onsuccess

            sw.Append("request.onupgradeneeded = function(event) {");
            sw.Append("var db = event.target.result;");
            sw.Append("var store = db.createObjectStore('clientmessages', {keyPath:'id'});");
            sw.Append("}; ");// end onupgraded


            sw.Append("},"); //end add message

            sw.Append("iterate: function(f) {");
            sw.Append("console.log('iterate message');");
            //sw.Append("console.log(messageBuffer);");
            //sw.Append("var i = messageBuffer.length;");
            //sw.Append("while (i--) {");
            //sw.Append("var msg = messageBuffer[i];");
            //sw.Append("f(msg);");
            //sw.Append("messageBuffer.splice(i, 1);");
            //sw.Append("}"); //end while


            sw.Append("var request = idb.open('sw_DB', 1);");

            sw.Append("request.onupgradeneeded = function(event) {");
            sw.Append("var db = event.target.result;");
            sw.Append("var store = db.createObjectStore('clientmessages', {keyPath : 'id'});");
            sw.Append("}; ");// end onupgraded


            sw.Append("request.onsuccess = function(event) {");

            sw.Append("var db = event.target.result; ");

            sw.Append("var transaction = db.transaction('clientmessages', 'readwrite');");
            sw.Append("transaction.onsuccess = function(event) {");
            sw.Append("console.log('[Transaction] ALL DONE!');");
            sw.Append("}; ");//endtransaction.success

            sw.Append("var msgStore = transaction.objectStore('clientmessages');");

            //get all messages and send to client
            sw.Append("msgStore.getAll().onsuccess = function(event) {");
            sw.Append("console.log('get all messages');");
            sw.Append("console.log(event.target.result);");
            sw.Append("var list = event.target.result;");

            sw.Append("list.forEach(function(msg) {");
            sw.Append("f(msg);");
            sw.Append("});");//end foreach

            sw.Append("msgStore.clear().onsuccess = function(event) {");
            sw.Append("console.log('clear all messages');");
            sw.Append("console.log(event);");
            sw.Append("}; "); //end clear


            sw.Append("}; "); //end getall
           

            sw.Append("}; "); //end request onsuccess

            



            sw.Append("}"); //end iterate
            sw.Append("};");// end  priv 
            sw.Append("} ");//end if priv
            sw.Append("} ");//end build function

            sw.Append("if(priv) {");
            sw.Append("console.log('returning ready priv');");
            sw.Append("return priv;");
            sw.Append("}");

            sw.Append("build();");
            sw.Append("return priv;");
            sw.Append("}; "); //end function

            sw.Append("if(!self.messageList) {");

            sw.Append("self.messageList = messageListBuilder(); ");
            sw.Append("} ");

            
            
            sw.Append("self.addEventListener('message', function(event){");
            sw.Append("console.log('SW Received Message ' + new Date().toString());");
            sw.Append("console.log(event);");

            sw.Append("if(event.data.type === 'page-ready') {");
            sw.Append("if(self.messageList) {");
            sw.Append("self.messageList.iterate(sendMessage);");

            //sw.Append("sendMessage({type:'hello' });");
            sw.Append("} ");


            sw.Append("}"); //endif windowready
           
            sw.Append("}); ");


            





            sw.AppendLine("self.importScripts('/js/push-notifications-controller.js');");
            
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

            sw.Append("let msg = { type:'cacheupdate', url: key, id: key }; ");

            sw.Append("if(self.messageList) {");
            sw.Append("self.messageList.addMessage(msg);");
            sw.Append("console.log('added new message ' + new Date().toString())");
            //sw.Append("self.messageList.iterate(sendMessage);");
            sw.Append("} ");

            //sw.Append("if(windowReady === true) {");
           // sw.Append("sendMessage(msg);");
            //sw.Append("} else {");
            //sw.Append("messageBuffer.push(msg);");
            //sw.Append("console.log(messageBuffer);");
            //sw.Append("}");

            
                
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
