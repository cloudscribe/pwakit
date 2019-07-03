using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace cloudscribe.PwaKit.Integration.Navigation.Controllers
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
