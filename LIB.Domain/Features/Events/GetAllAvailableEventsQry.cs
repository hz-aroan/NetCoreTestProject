using LIB.Domain.Contracts;
using LIB.Domain.Services.DTO;

using Microsoft.EntityFrameworkCore;

namespace LIB.Domain.Features.Events;

public sealed record GetAllAvailableEventsQry : IQueryRequest<IList<Services.DTO.Event>>;



public class GetEventsAvailableQryHandler : IQueryHandler<GetAllAvailableEventsQry, IList<Services.DTO.Event>>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetEventsAvailableQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }


    public async Task<IList<Event>> Handle(GetAllAvailableEventsQry request, CancellationToken cancellationToken)
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