using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIB.Domain.Contracts;
using LIB.Domain.Features.Events;
using LIB.Domain.Services.DTO;

namespace LIB.Domain.Services;

public class CurrencyHandlingService : ICurrencyHandlingService
{
    private static readonly IList<Currency> AvailableCurrencies = new[] {
        new Currency("€ Euro", "eur", "€"),
        new Currency("$ USD", "usd", "$"),
        new Currency("HUF Ft", "huf", "Ft")
    };

    private static readonly Dictionary<String, String> Currencies =
        AvailableCurrencies.ToDictionary(p => p.Id, p => p.Sign, StringComparer.InvariantCultureIgnoreCase);



    public IList<Currency> GetCurrencies()
    {
        return AvailableCurrencies;
    }



    public String GetSing(String currencyId)
    {
        if (string.IsNullOrEmpty(currencyId))
            return "";

        return Currencies.ContainsKey(currencyId)
            ? Currencies[currencyId]
            : "?";
    }
}