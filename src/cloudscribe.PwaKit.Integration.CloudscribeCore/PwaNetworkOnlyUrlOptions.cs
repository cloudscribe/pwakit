using System.Collections.Generic;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class PwaNetworkOnlyUrlOptions
    {
        /// <summary>
        /// a list of urls that result in the page being regarded as network only - never cached
        /// </summary>
        public PwaNetworkOnlyUrlOptions()
        {
            Urls = new List<string>();
        }

        public List<string> Urls { get; set; }
    }
}
