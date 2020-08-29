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

        [HttpGet("Switch")]
        public IActionResult Switch()
        {
            return View();
        }
    }
}
