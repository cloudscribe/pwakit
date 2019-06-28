using System;

namespace cloudscribe.PwaKit.Models
{
    public class PushSubscription : Lib.Net.Http.WebPush.PushSubscription
    {

        //this was copied from https://github.com/tpeczek/Demo.AspNetCore.PushNotifications
        // no idea what these 2 properties are for maybe not needed

        public string P256DH
        {
            get { return GetKey(Lib.Net.Http.WebPush.PushEncryptionKeyName.P256DH); }

            set { SetKey(Lib.Net.Http.WebPush.PushEncryptionKeyName.P256DH, value); }
        }

        public string Auth
        {
            get { return GetKey(Lib.Net.Http.WebPush.PushEncryptionKeyName.Auth); }

            set { SetKey(Lib.Net.Http.WebPush.PushEncryptionKeyName.Auth, value); }
        }

        /// <summary>
        /// I'm not sure we ever want to use tenant identifiers as we probably only want 1 queue for all tenants
        /// but for nodb we need at least "default" so they will go in default project folder
        /// </summary>
        public string TenantId { get; set; } = "default";

        public string UserId { get; set; }

        public Guid Key { get; set; }

        public PushSubscription()
        {

        }

        public PushSubscription(Lib.Net.Http.WebPush.PushSubscription subscription)
        {
            Key = Guid.NewGuid();
            Endpoint = subscription.Endpoint;
            Keys = subscription.Keys;
        }
    }
}
