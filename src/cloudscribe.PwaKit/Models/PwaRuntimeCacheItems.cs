using System.Collections.Generic;

namespace cloudscribe.PwaKit.Models
{
    public class PwaRuntimeCacheItems
    {
        public PwaRuntimeCacheItems()
        {
            Assets = new List<ServiceWorkerCacheItem>();
        }

        public List<ServiceWorkerCacheItem> Assets { get; set; }

    }
}
