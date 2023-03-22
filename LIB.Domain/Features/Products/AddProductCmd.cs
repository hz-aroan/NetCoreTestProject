using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Exceptions;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;

using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Products;

public class AddProductCmd : ICommandArg
{
    internal readonly String Currency;

    internal readonly Double FeeAmount;

    internal readonly Boolean IsAvailable;

    internal readonly String ProductName;



    public AddProductCmd(String productName, Double feeAmount, String currency, Boolean isAvailable)
    {
        FeeAmount = feeAmount;
        Currency = currency;
        ProductName = productName;
        IsAvailable = isAvailable;
    }
}

public class AddProductCmdHandler : ICommandHandler<AddProductCmd>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public AddProductCmdHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }


    public void Execute(AddProductCmd cmd)
    {
        var product = new Product
        {
            Name = cmd.ProductName,
            IsAvailable = cmd.IsAvailable,
            FeeAmount = (Decimal)cmd.FeeAmount,
            FeeCurrency = cmd.Currency
        };

        var validator = new ProductValidatingService(CurrencyService);
        validator.ValidateNewOne(product);

        using var ctx = EFWrapper.GetContext();
        ctx.Products.Add(product);
        EFWrapper.SafeFinish(ctx, "An error happened adding the new product!");
    }
}