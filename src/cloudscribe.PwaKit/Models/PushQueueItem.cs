using System;

namespace cloudscribe.PwaKit.Models
{
    public class PushQueueItem
    {
        public PushQueueItem(
            PushMessageModel message,
            string recipientProviderName
            )
        {
            if (string.IsNullOrEmpty(recipientProviderName)) throw new ArgumentException("You must provide a recipient provider name");

            Message = message ?? throw new ArgumentNullException("You must provide a PushMessage");
            RecipientProviderName = recipientProviderName;
        }

        public string TenantId { get; set; } = "default";

        public string RecipientProviderName { get; private set; }

        public string RecipientProviderCustom1 { get; set; }
        public string RecipientProviderCustom2 { get; set; }
        public string RecipientProviderCustom3 { get; set; }
        public string RecipientProviderCustom4 { get; set; }

        public PushMessageModel Message { get; private set; }

    }
}
