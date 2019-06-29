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

            sw.Append("const pushNotificationTitle = 'PwaKit Demo';");

            sw.Append("self.addEventListener('push', function (event) {");

            sw.Append("console.log(event);");
            //sw.Append("var json = JSON.parse(event.data.text());");
            sw.Append("var json = event.data.json();");
            sw.Append("console.log(json);");

            //sw.Append("event.waitUntil(self.registration.showNotification(json.title, {");
            //sw.Append("body: json.body,");
            //sw.Append("icon: json.icon");
            //sw.Append("}));");
            //sw.Append("});");

            sw.Append("event.waitUntil(self.registration.showNotification(json.title, json");
            sw.Append("));");

            sw.Append("});");




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
