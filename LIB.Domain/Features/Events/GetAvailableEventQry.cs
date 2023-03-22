using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Exceptions;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using Event = LIB.Domain.Services.DTO.Event;

namespace LIB.Domain.Features.Events;

public class GetAvailableEventQry : IQueryRequest<Event>
{
    internal readonly Int32 EventId;



    public GetAvailableEventQry(Int32 eventId)
    {
        EventId = eventId;
    }
}

public class GetAvailableEventQryHandler : IQueryHandler<GetAvailableEventQry, Event>
{
    private readonly CurrencyHandlingService CurrencyService;

    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public GetAvailableEventQryHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
        CurrencyService = new CurrencyHandlingService();
    }



    public Event Execute(GetAvailableEventQry queryArg)
    {
        using var ctx = DbctxFactory.CreateDbContext();

        var rawEvent = ctx.Events.FirstOrDefault(p => p.IsAvailable && p.EventId == queryArg.EventId);
        if (rawEvent == null)
            throw new DomainException($"Event - id={queryArg.EventId} - is unknown!");

        var result = new Event(rawEvent, CurrencyService.GetSing(rawEvent.FeeCurrency));
        return result;
    }
}