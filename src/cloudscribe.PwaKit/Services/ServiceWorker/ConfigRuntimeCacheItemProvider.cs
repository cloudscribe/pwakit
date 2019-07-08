using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Services
{
    public class ConfigRuntimeCacheItemProvider : IRuntimeCacheItemProvider
    {
        public ConfigRuntimeCacheItemProvider(IOptions<PwaRuntimeCacheItems> optionsAccessor)
        {
            _runtimeCacheItems = optionsAccessor.Value;
        }

        private readonly PwaRuntimeCacheItems _runtimeCacheItems;

        public Task<List<ServiceWorkerCacheItem>> GetItems()
        {
            return Task.FromResult(_runtimeCacheItems.Assets);
        }


    }
}
