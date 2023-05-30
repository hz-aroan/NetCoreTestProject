using LIB.Domain.Contracts;
using LIB.Domain.Services.DTO;

using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Events;

public sealed record GetAllEventsQry : IQueryRequest<IList<Event>>;



public class GetAllEventsQryHandler : IQueryHandler<GetAllEventsQry, IList<Event>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetAllEventsQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public async Task<IList<Event>> Handle(GetAllEventsQry request, CancellationToken cancellationToken)
    {
        using var ctx = EFWrapper.GetContext();
        var rawEvents = ctx.Events.OrderBy(p => p.EventId).AsNoTracking();

        var result = rawEvents.Select(p => new Event(p, CurrencyService.GetSing(p.FeeCurrency)));
        return result.ToList();
    }
}