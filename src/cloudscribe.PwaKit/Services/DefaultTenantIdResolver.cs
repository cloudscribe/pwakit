using cloudscribe.PwaKit.Interfaces;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultTenantIdResolver : ITenantIdResolver
    {
        public string GetTenantId()
        {
            return "default";
        }

    }
}
