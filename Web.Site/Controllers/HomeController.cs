using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Web.Site.Models;

namespace Web.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            throw new NotImplementedException("not implemented controller method");
        }
    }
}