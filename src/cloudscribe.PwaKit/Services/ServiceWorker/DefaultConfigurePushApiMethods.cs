using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultConfigurePushApiMethods : IConfigurePushApiMethods
    {
        public DefaultConfigurePushApiMethods()
        {

        }

        public Task AppendToInitScript(
            StringBuilder script, 
            HttpContext context, 
            IUrlHelper urlHelper
            )
        {
            script.Append("const PushNotificationsController = (function () {");
            
            script.Append("function urlB64ToUint8Array(base64String) {");
            script.Append("const padding = '='.repeat((4 - base64String.length % 4) % 4);");
            script.Append("const base64 = (base64String + padding).replace(/\\-/g, '+').replace(/_/g, '/');");
            // can this line work inside serviceworker reference window?
            script.Append("const rawData = window.atob(base64);");
            script.Append("const outputArray = new Uint8Array(rawData.length);");
            script.Append("for (let i = 0; i < rawData.length; ++i) {");
            script.Append("outputArray[i] = rawData.charCodeAt(i);");
            script.Append("}");
            script.Append(" return outputArray;");
            script.Append("} ");
            
            script.Append(" return {");

            script.Append("retrievePublicKey: function () {");
            script.Append("return fetch('/pwa/getpublickey').then(function (response) {");
            script.Append("if (response.ok) {");
            script.Append("return response.text().then(function (applicationServerPublicKeyBase64) {");
            script.Append("return urlB64ToUint8Array(applicationServerPublicKeyBase64);");
            script.Append("});");
            script.Append("} else {");
            script.Append("return Promise.reject(response.status + ' ' + response.statusText);");
            script.Append("}");
            script.Append("});");
            script.Append("},");
            
            
            script.Append("storePushSubscription: function (pushSubscription) {");
            script.Append("return fetch('/pwa/subscription', {");
            script.Append("method: 'POST',");
            script.Append("headers: { 'Content-Type': 'application/json' },");
            script.Append("body: JSON.stringify(pushSubscription)");
            script.Append("});");
            script.Append("},");

            script.Append("discardPushSubscription: function (pushSubscription) {");
            script.Append("return fetch('/pwa/subscription?endpoint=' + encodeURIComponent(pushSubscription.endpoint), {");
            script.Append("method: 'DELETE'");
            script.Append("});");
            script.Append("}");

            script.Append("};");

            script.Append("})();");


            return Task.CompletedTask;
        }

    }
}
