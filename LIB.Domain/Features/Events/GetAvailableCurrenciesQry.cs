using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIB.Domain.Contracts;
using LIB.Domain.Services;
using LIB.Domain.Services.CQ;
using LIB.Domain.Services.DTO;

namespace LIB.Domain.Features.Events;

public class GetAvailableCurrenciesQry : IQueryRequest<IList<Currency>>
{
}

public class GetAvailableCurrenciesQryHandler : IQueryHandler<GetAvailableCurrenciesQry, IList<Currency>>
{
    private readonly ICurrencyHandlingService CurrencyService;


    public GetAvailableCurrenciesQryHandler(ICurrencyHandlingService currencyService)
    {
        CurrencyService = currencyService;
    }



    public IList<Currency> Execute(GetAvailableCurrenciesQry request)
    {
        return CurrencyService.GetCurrencies();
    }
}