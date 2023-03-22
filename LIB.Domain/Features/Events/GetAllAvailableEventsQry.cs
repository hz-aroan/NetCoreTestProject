using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Events;

public class GetAllAvailableEventsQry : IQueryRequest<IList<Services.DTO.Event>>
{
}

public class GetEventsAvailableQryHandler : IQueryHandler<GetAllAvailableEventsQry, IList<Services.DTO.Event>>
{
    private readonly CurrencyHandlingService CurrencyService;

    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetEventsAvailableQryHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
        CurrencyService = new CurrencyHandlingService();
    }



    public IList<Services.DTO.Event> Execute(GetAllAvailableEventsQry queryArg)
    {
        using var ctx = DbctxFactory.CreateDbContext();
        var rawEvents = ctx.Events.Where(p => p.IsAvailable)
            .OrderBy(p => p.EventId)
            .AsNoTracking();
        var result = rawEvents.Select(p => new Services.DTO.Event(p, CurrencyService.GetSing(p.FeeCurrency)));
        return result.ToList();
    }
}