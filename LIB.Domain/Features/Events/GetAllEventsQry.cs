using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using Event = LIB.Domain.Services.DTO.Event;

namespace LIB.Domain.Features.Events;

public class GetAllEventsQry : IQueryRequest<IList<Event>>
{
}

public class GetAllEventsQryHandler : IQueryHandler<GetAllEventsQry, IList<Event>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetAllEventsQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public IList<Event> Execute(GetAllEventsQry queryArg)
    {
        using var ctx = EFWrapper.GetContext();
        var rawEvents = ctx.Events.OrderBy(p => p.EventId)
            .AsNoTracking();

        var result = rawEvents.Select(p => new Event(p, CurrencyService.GetSing(p.FeeCurrency)));
        return result.ToList();
    }
}