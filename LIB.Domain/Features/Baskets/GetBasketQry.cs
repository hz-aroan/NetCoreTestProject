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
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetAllProductsOfBasketQryHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public Basket Execute(GetBasketQry queryArg)
    {
        var service = new BasketHandlingService(DbctxFactory);
        var result = service.GetBasket(queryArg.BasketUid);
        return result;
    }
}