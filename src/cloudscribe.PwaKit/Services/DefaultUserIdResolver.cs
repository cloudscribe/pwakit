using cloudscribe.PwaKit.Interfaces;
using System.Security.Claims;

namespace cloudscribe.PwaKit.Services
{
    public class DefaultUserIdResolver : IUserIdResolver
    {
        public string GetUserId(ClaimsPrincipal user)
        {
            if(user.Identity.IsAuthenticated)
            {
                return user.Identity.Name;
            }

            return null;
        }

    }
}
