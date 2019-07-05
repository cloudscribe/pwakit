using Microsoft.AspNetCore.Mvc;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore.Controllers
{
    public class PwaNavController : Controller
    {
        public PwaNavController()
        {

        }

        [HttpGet]
        [HttpHead]
        public IActionResult TopNav(string currentUrl)
        {

            return PartialView("TopNavPartial", currentUrl);
        }


    }
}
