using LIB.Domain.Contracts;
using LIB.Domain.Exceptions;
using LIB.Domain.Services.DTO;

namespace LIB.Domain.Features.Events;

public sealed record GetAvailableEventQry(Int32 EventId) : IQueryRequest<Event>;



public class GetAvailableEventQryHandler : IQueryHandler<GetAvailableEventQry, Event>
{
    private readonly ICurrencyHandlingService CurrencyService;

    private readonly IEFWrapper EFWrapper;



    public GetAvailableEventQryHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }



    public async Task<Event> Handle(GetAvailableEventQry queryArg, CancellationToken cancellationToken)
    {
        using var ctx = EFWrapper.GetContext();

        var rawEvent = ctx.Events.FirstOrDefault(p => p.IsAvailable && p.EventId == queryArg.EventId);
        if (rawEvent == null)
            throw new DomainException($"Event - id={queryArg.EventId} - is unknown!");

        var result = new Event(rawEvent, CurrencyService.GetSing(rawEvent.FeeCurrency));
        return result;
    }
}