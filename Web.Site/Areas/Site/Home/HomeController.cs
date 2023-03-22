using LIB.Domain.Services.CQ;
using Microsoft.AspNetCore.Mvc;

namespace Web.Site.Areas.Site.Home;

[Area("Site")]
public class HomeController : Controller
{
    private readonly HomeFactory Factory;



    public HomeController(ILogger<HomeController> logger, IDispatcher dispatcher)
    {
        Factory = new HomeFactory(this, logger, dispatcher);
    }



    public IActionResult Index()
    {
        var model = Factory.GetIndexModel();
        return View("Index", model);
    }
}