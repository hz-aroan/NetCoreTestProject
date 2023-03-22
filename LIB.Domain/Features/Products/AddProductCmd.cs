using Infrastructure.SQL.Main;
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
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public AddProductCmdHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public void Execute(AddProductCmd cmd)
    {
        var product = new Product {
            Name = cmd.ProductName,
            IsAvailable = cmd.IsAvailable,
            FeeAmount = (Decimal)cmd.FeeAmount,
            FeeCurrency = cmd.Currency
        };

        var availableCurrencies = CurrencyHandlingService.AvailableCurrencies;

        var validator = new ProductValidatingService();
        validator.ValidateNewOne(product, availableCurrencies);

        using var ctx = DbctxFactory.CreateDbContext();
        ctx.Products.Add(product);

        try
        {
            ctx.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DomainException("An error happened adding the new product!", ex);
        }
    }
}