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

public class CreateBasketCmd : IQueryRequest<Guid>
{
    internal readonly Int32 EventId;



    public CreateBasketCmd(Int32 eventId)
    {
        EventId = eventId;
    }
}

public class GetNewShoppingCartCmdHandler : IQueryHandler<CreateBasketCmd, Guid>
{
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetNewShoppingCartCmdHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public Guid Execute(CreateBasketCmd queryArg)
    {
        var service = new BasketHandlingService(DbctxFactory);
        var result = service.CreateNewBasket(queryArg.EventId);
        return result;
    }
}