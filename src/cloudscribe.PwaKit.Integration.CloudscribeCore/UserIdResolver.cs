using cloudscribe.Core.Identity;
using cloudscribe.PwaKit.Interfaces;
using System.Security.Claims;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class UserIdResolver : IUserIdResolver
    {

        public string GetUserId(ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                return user.GetUserIdAsGuid().ToString();
            }

            return null;
        }

    }
}
