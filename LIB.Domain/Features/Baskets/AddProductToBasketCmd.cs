using Infrastructure.SQL.Main;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.Domain.Contracts;

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
    private readonly IBasketHandlingService BasketHandler;


    public AddProductToBasketCmdHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }



    public void Execute(AddProductToBasketCmd queryArg)
    {
        BasketHandler.AddProductToBasket(queryArg.BasketUid, queryArg.ProductId, queryArg.Quantity);
    }
}