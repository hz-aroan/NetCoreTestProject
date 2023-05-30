using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Services;

namespace LIB.Domain.Features.Products;

public sealed record AddProductCmd(String ProductName, Double FeeAmount, String Currency, Boolean IsAvailable) : ICommandArg;



public class AddProductCmdHandler : ICommandHandler<AddProductCmd>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public AddProductCmdHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }


    public Task Handle(AddProductCmd request, CancellationToken cancellationToken)
    {
         var product = new Product
        {
            Name = request.ProductName,
            IsAvailable = request.IsAvailable,
            FeeAmount = (Decimal)request.FeeAmount,
            FeeCurrency = request.Currency
        };

        var validator = new ProductValidatingService(CurrencyService);
        validator.ValidateNewOne(product);

        using var ctx = EFWrapper.GetContext();
        ctx.Products.Add(product);
        EFWrapper.SafeFinish(ctx, "An error happened adding the new product!");
        return Task.CompletedTask;
   }
}