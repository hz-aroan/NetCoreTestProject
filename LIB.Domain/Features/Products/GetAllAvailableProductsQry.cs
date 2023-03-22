using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Contracts;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using LIB.Domain.Services.DTO;
using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Products;

public class GetAllAvailableProductsQry : IQueryRequest<IList<Services.DTO.Product>>
{
}

public class GetAllAvailableProductsQryHandler : IQueryHandler<GetAllAvailableProductsQry, IList<Services.DTO.Product>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetAllAvailableProductsQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public IList<Services.DTO.Product> Execute(GetAllAvailableProductsQry queryArg)
    {
        using var ctx = EFWrapper.GetContext();
        var rawEvents = ctx.Products.IgnoreAutoIncludes()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name);
        var result = rawEvents.Select(CreateProduct);
        return result.ToList();
    }



    private Services.DTO.Product CreateProduct(Infrastructure.SQL.Main.Product p)
    {
        var sign = CurrencyService.GetSing(p.FeeCurrency);
        var result = new Services.DTO.Product(p, sign);
        return result;
    }
}