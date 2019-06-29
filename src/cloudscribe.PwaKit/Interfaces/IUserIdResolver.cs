using System.Security.Claims;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IUserIdResolver
    {
        string GetUserId(ClaimsPrincipal user);
    }
}
