using System;
using System.Collections.Generic;
using System.Text;

namespace cloudscribe.PwaKit.Models
{
    public class PwaPreCacheItems
    {
        public PwaPreCacheItems()
        {
            Assets = new List<ServiceWorkerCacheItem>();
        }

        public List<ServiceWorkerCacheItem> Assets { get; set; }

    }
}
