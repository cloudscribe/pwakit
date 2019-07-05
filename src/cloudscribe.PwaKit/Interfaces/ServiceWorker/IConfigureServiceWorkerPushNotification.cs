using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IConfigureServiceWorkerPushNotification
    {
        Task AppendToServiceWorkerScript(StringBuilder sw, PwaOptions options, HttpContext context, IUrlHelper urlHelper);
    }
}
