using cloudscribe.Core.Identity;
using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class UserIdResolver : IUserIdResolver
    {
        public UserIdResolver(IHttpContextAccessor httpContextAccessor)
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
                    return context.User.GetUserIdAsGuid().ToString();
                }
            }
            

            return null;
        }

    }
}
