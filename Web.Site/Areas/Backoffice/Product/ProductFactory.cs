using LIB.Domain.Contracts;
using LIB.Domain.Features.Events;
using LIB.Domain.Features.Products;

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
        var result = new ProductSelectionViewModel
        {
            Products = Dispatcher.Send(new GetAllAvailableProductsQry()).Result
        };
        return result;
    }



    public AddProductViewModel GetAddProductModel()
    {
        var availableCurrencies = Dispatcher.Send(new GetAvailableCurrenciesQry()).Result;
        var result = new AddProductViewModel
        {
            Currencies = availableCurrencies.Select(p => new SelectListItem { Value = p.Id, Text = p.Text }).ToList(),
            Currency = availableCurrencies.First().Id
        };
        return result;
    }



    public async Task<Boolean> ProcessAddProduct(ProductForm form)
    {
        await Dispatcher.Send(new AddProductCmd(form.ProductName, form.FeeAmount, form.Currency, true));
        return true;
    }
}