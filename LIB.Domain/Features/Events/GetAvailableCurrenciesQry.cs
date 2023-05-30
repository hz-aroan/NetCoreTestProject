using LIB.Domain.Contracts;
using LIB.Domain.Services.DTO;

namespace LIB.Domain.Features.Events;

public sealed record GetAvailableCurrenciesQry : IQueryRequest<IList<Currency>>;



public class GetAvailableCurrenciesQryHandler : IQueryHandler<GetAvailableCurrenciesQry, IList<Currency>>
{
    private readonly ICurrencyHandlingService CurrencyService;


    public GetAvailableCurrenciesQryHandler(ICurrencyHandlingService currencyService)
    {
        CurrencyService = currencyService;
    }



    public async Task<IList<Currency>> Handle(GetAvailableCurrenciesQry request, CancellationToken cancellationToken)
    {
        return CurrencyService.GetCurrencies();
    }
}