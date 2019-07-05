using cloudscribe.Web.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore.Controllers
{
    public class PwaNavController : Controller
    {
        public PwaNavController(IResourceHelper resourceHelper)
        {
            _resourceHelper = resourceHelper;
        }

        private readonly IResourceHelper _resourceHelper;

        [HttpGet]
        [HttpHead]
        public IActionResult TopNav(string currentUrl)
        {

            return PartialView("TopNavPartial", currentUrl);
        }


        private IActionResult GetResult(string resourceName, string contentType)
        {
            var assembly = typeof(PwaNavController).GetTypeInfo().Assembly;
            resourceName = _resourceHelper.ResolveResourceIdentifier(resourceName);
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream == null)
            {
                //Log.LogError("resource not found for " + resourceName);
                return NotFound();
            }

            //Log.LogDebug("resource found for " + resourceName);

            var status = ETagGenerator.AddEtagForStream(HttpContext, resourceStream);
            if (status != null) { return status; } //304

            return new FileStreamResult(resourceStream, contentType);
        }

        // /pwanav/js/
        [HttpGet]
        [Route("pwanav/js/{slug}")]
        [AllowAnonymous]
        public virtual IActionResult Js()
        {
            var baseSegment = "cloudscribe.PwaKit.Integration.CloudscribeCore.js.";

            var requestPath = HttpContext.Request.Path.Value;
            //Log.LogDebug(requestPath + " requested");

            if (requestPath.Length < "/pwanav/js/".Length) return NotFound();

            var seg = requestPath.Substring("/pwanav/js/".Length);
            var ext = Path.GetExtension(requestPath);
            var mimeType = _resourceHelper.GetMimeType(ext);

            return GetResult(baseSegment + seg, mimeType);
        }


    }
}
