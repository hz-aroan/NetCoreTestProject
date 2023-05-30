using LIB.Domain.Contracts;

using Microsoft.AspNetCore.Mvc;

using Web.Site.Areas.Backoffice.Event;
using Web.Site.Areas.Backoffice.Product.Models;

namespace Web.Site.Areas.Backoffice.Product;

[Area("Backoffice")]
public class ProductController : Controller
{
    private readonly ProductFactory Factory;



    public ProductController(ILogger<EventController> logger, IDispatcher dispatcher)
    {
        Factory = new ProductFactory(this, logger, dispatcher);
    }



    public IActionResult Index()
    {
        var model = Factory.GetIndexModel();
        return View("Index", model);
    }



    public IActionResult AddProduct()
    {
        var model = Factory.GetAddProductModel();
        return PartialView("_AddNewProduct", model);
    }



    [HttpPost]
    public async Task<IActionResult> DoAddProduct(ProductForm form)
    {
        var model = await Factory.ProcessAddProduct(form);
        return Redirect("Index");
    }
}