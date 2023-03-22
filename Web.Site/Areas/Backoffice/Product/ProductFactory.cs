using LIB.Domain.Features.Events;
using LIB.Domain.Features.Products;
using LIB.Domain.Services.CQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Site.Areas.Backoffice.Product.Models;

namespace Web.Site.Areas.Backoffice.Product;

public class ProductFactory
{
    private readonly IDispatcher Dispatcher;

    private readonly ILogger Logger;

    private readonly Controller Parent;



    public ProductFactory(Controller parent, ILogger logger, IDispatcher dispatcher)
    {
        Parent = parent;
        Logger = logger;
        Dispatcher = dispatcher;
    }



    public ProductSelectionViewModel GetIndexModel()
    {
        var result = new ProductSelectionViewModel {
            Products = Dispatcher.Query(new GetAllAvailableProductsQry())
        };
        return result;
    }



    public AddProductViewModel GetAddProductModel()
    {
        var availableCurrencies = Dispatcher.Query(new GetAvailableCurrenciesQry());
        var result = new AddProductViewModel {
            Currencies = availableCurrencies.Select(p => new SelectListItem { Value = p.Id, Text = p.Text })
                .ToList(),
            Currency = availableCurrencies.First()
                .Id
        };
        return result;
    }



    public Boolean ProcessAddProduct(ProductForm form)
    {
        Dispatcher.Execute(new AddProductCmd(form.ProductName, form.FeeAmount, form.Currency, true));
        return true;
    }
}