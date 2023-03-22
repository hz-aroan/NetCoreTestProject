using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.AspNetCore.Mvc;

namespace Web.Site.Areas.Site.Shopping;

[Area("Site")]
public class ShoppingController : Controller
{
    private readonly ShoppingFactory Factory;



    public ShoppingController(ILogger<ShoppingController> logger, IDispatcher dispatcher)
    {
        Factory = new ShoppingFactory(this, logger, dispatcher);
    }



    public IActionResult Index()
    {
        var model = Factory.GetIndexModel();
        if (model == null) return RedirectToAction("Index", "Home", new { Area = "Site" });
        return View("Index", model);
    }



    public IActionResult SelectEvent(Int32 eventId)
    {
        Factory.SelectEvent(eventId);
        return Redirect("Index");
    }



    public IActionResult AddProduct(Int32 productId, Int32 qty)
    {
        Factory.AddProductToEvent(productId, qty);
        return Redirect("Index");
    }



    public IActionResult GetProducts()
    {
        var model = Factory.GetProducts();
        return PartialView("_ProductsList", model);
    }
}