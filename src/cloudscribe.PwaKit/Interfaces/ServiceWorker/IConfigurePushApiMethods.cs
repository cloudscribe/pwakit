using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IConfigurePushApiMethods
    {
        Task AppendToInitScript(StringBuilder script, HttpContext context, IUrlHelper urlHelper);
    }
}
