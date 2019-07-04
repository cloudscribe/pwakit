using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IRuntimeCacheItemProvider
    {
        Task<List<ServiceWorkerCacheItem>> GetItems();
    }
}
