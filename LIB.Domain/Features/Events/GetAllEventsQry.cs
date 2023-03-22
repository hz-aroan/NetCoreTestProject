using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
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
    private readonly CurrencyHandlingService CurrencyService;

    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetAllEventsQryHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
        CurrencyService = new CurrencyHandlingService();
    }



    public IList<Event> Execute(GetAllEventsQry queryArg)
    {
        using var ctx = DbctxFactory.CreateDbContext();
        var rawEvents = ctx.Events.OrderBy(p => p.EventId)
            .AsNoTracking();
        var result = rawEvents.Select(p => new Event(p, CurrencyService.GetSing(p.FeeCurrency)));
        return result.ToList();
    }
}