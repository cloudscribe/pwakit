using System;

namespace cloudscribe.PwaKit.Models
{
    public class PushDeviceSubscription : Lib.Net.Http.WebPush.PushSubscription
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

       
        public string TenantId { get; set; } = "default";

        public string UserId { get; set; }

        public string UserAgent { get; set; }

        public string CreatedFromIpAddress { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public Guid Key { get; set; }

        public PushDeviceSubscription()
        {

        }

        public PushDeviceSubscription(Lib.Net.Http.WebPush.PushSubscription subscription)
        {
            Key = Guid.NewGuid();
            Endpoint = subscription.Endpoint;
            Keys = subscription.Keys;
        }
    }
}
