using Infrastructure.SQL.Main;
using LIB.Domain.Services.CQ;
using LIB.Domain.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.Domain.Exceptions;
using LIB.Domain.Services;
using LIB.Domain.Contracts;

namespace LIB.Domain.Features.Baskets;

public class GetBasketQry : IQueryRequest<Basket>
{
    internal readonly Guid BasketUid;



    public GetBasketQry(Guid basketUid)
    {
        BasketUid = basketUid;
    }
}


public class GetAllProductsOfBasketQryHandler : IQueryHandler<GetBasketQry, Basket>
{
    private readonly IBasketHandlingService BasketHandler;


    public GetAllProductsOfBasketQryHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }



    public Basket Execute(GetBasketQry queryArg)
    {
        var result = BasketHandler.GetBasket(queryArg.BasketUid);
        return result;
    }
}