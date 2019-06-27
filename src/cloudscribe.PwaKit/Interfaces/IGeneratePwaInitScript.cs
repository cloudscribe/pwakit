using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IGeneratePwaInitScript
    {
        Task<string> BuildPwaInitScript(HttpContext context, IUrlHelper urlHelper);
    }
}
