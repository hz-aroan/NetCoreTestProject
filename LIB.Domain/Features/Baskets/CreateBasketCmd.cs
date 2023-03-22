using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
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
    private readonly IBasketHandlingService BasketHandler;



    public GetNewShoppingCartCmdHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }



    public Guid Execute(CreateBasketCmd queryArg)
    {
        var result = BasketHandler.CreateNewBasket(queryArg.EventId);
        return result;
    }
}