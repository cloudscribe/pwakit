using cloudscribe.Core.Models;
using cloudscribe.PwaKit.Interfaces;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class TenantIdResolver : ITenantIdResolver
    {
        public TenantIdResolver(SiteContext siteContext)
        {
            _siteContext = siteContext;
        }

        private readonly SiteContext _siteContext;

        public string GetTenantId()
        {
            return _siteContext.Id.ToString();
        }

    }
}
