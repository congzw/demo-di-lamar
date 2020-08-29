using System;
using Microsoft.AspNetCore.Mvc;
using NbSites.Web.MultiTenancy;

namespace NbSites.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Render/{view=Index}")]
        public IActionResult Render(string view)
        {
            return View(view);
        }

        [HttpGet("Switch")]
        public IActionResult Switch()
        {
            return View();
        }
    }
}
