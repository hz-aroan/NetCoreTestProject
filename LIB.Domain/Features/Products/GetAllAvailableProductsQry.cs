using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
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
    private static readonly CurrencyHandlingService CurrencyService = new CurrencyHandlingService();

    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetAllAvailableProductsQryHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public IList<Services.DTO.Product> Execute(GetAllAvailableProductsQry queryArg)
    {
        using var ctx = DbctxFactory.CreateDbContext();
        var rawEvents = ctx.Products.IgnoreAutoIncludes()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name);
        var result = rawEvents.Select(p => CreateProduct(p));
        return result.ToList();
    }



    private static Services.DTO.Product CreateProduct(Infrastructure.SQL.Main.Product p)
    {
        var sign = CurrencyService.GetSing(p.FeeCurrency);
        var result = new Services.DTO.Product(p, sign);
        return result;
    }
}