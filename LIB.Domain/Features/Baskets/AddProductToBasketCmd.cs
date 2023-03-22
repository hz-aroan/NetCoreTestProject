using Infrastructure.SQL.Main;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Features.Baskets;

public class AddProductToBasketCmd : ICommandArg
{
    internal readonly Guid BasketUid;

    internal readonly Int32 ProductId;

    internal readonly Int32 Quantity;



    public AddProductToBasketCmd(Guid basketUid, Int32 productId, Int32 quantity)
    {
        BasketUid = basketUid;
        ProductId = productId;
        Quantity = quantity;
    }
}

public class AddProductToBasketCmdHandler : ICommandHandler<AddProductToBasketCmd>
{
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public AddProductToBasketCmdHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public void Execute(AddProductToBasketCmd queryArg)
    {
        var service = new BasketHandlingService(DbctxFactory);
        service.AddProductToBasket(queryArg.BasketUid, queryArg.ProductId, queryArg.Quantity);
    }
}