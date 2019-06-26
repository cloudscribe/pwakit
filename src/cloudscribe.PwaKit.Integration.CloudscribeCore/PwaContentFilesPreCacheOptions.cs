using System;
using System.Collections.Generic;
using System.Text;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class PwaContentFilesPreCacheOptions
    {

        /// <summary>
        /// a csv of file extension patterns to cache in format ".jpg,.gif,.png,.jpe,.jpeg"
        /// </summary>
        public string FileExtensionsToCache { get; set; } = ".jpg,.gif,.png,.jpe,.jpeg";
    }
}
