using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultUserIdResolver : IUserIdResolver
    {
        public DefaultUserIdResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public string GetCurrentUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            if(context != null)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    return context.User.Identity.Name;
                }
            }
            

            return null;
        }

    }
}
