using LIB.Domain.Contracts;
using LIB.Domain.Services.DTO;

using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Products;

public sealed record GetAllAvailableProductsQry : IQueryRequest<IList<Services.DTO.Product>>;




public class GetAllAvailableProductsQryHandler : IQueryHandler<GetAllAvailableProductsQry, IList<Services.DTO.Product>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetAllAvailableProductsQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public async Task<IList<Product>> Handle(GetAllAvailableProductsQry request, CancellationToken cancellationToken)
    {
        using var ctx = EFWrapper.GetContext();
        var rawEvents = ctx.Products.IgnoreAutoIncludes()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.Name);
        var result = rawEvents.Select(CreateProduct);
        return result.ToList();
    }



    private Product CreateProduct(Infrastructure.SQL.Main.Product p)
    {
        var sign = CurrencyService.GetSing(p.FeeCurrency);
        var result = new Product(p, sign);
        return result;
    }
}