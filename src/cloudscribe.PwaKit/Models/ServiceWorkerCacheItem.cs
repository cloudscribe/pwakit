using System;

namespace cloudscribe.PwaKit.Models
{
    public class ServiceWorkerCacheItem
    {
        public string Url { get; set; }
        public string Revision { get; set; }

        public DateTime? LastModifiedUtc { get; set; }
    }
}
