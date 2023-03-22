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

namespace LIB.Domain.Features.Events;

public class GetAllAvailableEventsQry : IQueryRequest<IList<Services.DTO.Event>>
{
}

public class GetEventsAvailableQryHandler : IQueryHandler<GetAllAvailableEventsQry, IList<Services.DTO.Event>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetEventsAvailableQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public IList<Services.DTO.Event> Execute(GetAllAvailableEventsQry queryArg)
    {
        using var ctx = EFWrapper.GetContext();
        var rawEvents = ctx.Events
            .IgnoreAutoIncludes()
            .Where(p => p.IsAvailable)
            .OrderBy(p => p.EventId)
            .AsNoTracking();
        
        var result = rawEvents.Select(p => new Services.DTO.Event(p, CurrencyService.GetSing(p.FeeCurrency)));
        return result.ToList();
    }
}