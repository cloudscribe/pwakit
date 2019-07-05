using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IServiceWorkerBuilder
    {
        Task<string> Build(HttpContext context, IUrlHelper urlHelper);
    }
}
