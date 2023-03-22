using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.SQL.Main;
using LIB.Domain.Contracts;
using LIB.Domain.Exceptions;
using LIB.Domain.Services.DTO;
using Event = Infrastructure.SQL.Main.Event;

namespace LIB.Domain.Services;

public class EventValidatingService
{
    private const Decimal MAX_FEE_AMOUNT = 1000;

    private readonly ICurrencyHandlingService CurrencyService;



    public EventValidatingService(ICurrencyHandlingService currencyHandlingService)
    {
        CurrencyService = currencyHandlingService;
    }


    public void ValidateNewOne(Event ev, MainDbContext ctx)
    {
        if (string.IsNullOrEmpty(ev.Name))
            throw new DomainException("The event name should not be empty!");

        var existingOne = ctx.Events.FirstOrDefault(p => p.Name == ev.Name);
        if (existingOne != null)
            throw new DomainException($"An event with this name '{ev.Name}' already exists!");

        if (ev.FeeAmount<0 )
            throw new DomainException("The event service fee should not be negative!");

        if (ev.FeeAmount > MAX_FEE_AMOUNT)
            throw new DomainException($"Invalid fee amount '{ev.FeeAmount}' - should not be greater the the maximum allowed value {MAX_FEE_AMOUNT}!");

        if (String.IsNullOrEmpty(ev.FeeCurrency))
            throw new DomainException($"The fee currency - {ev.FeeCurrency} - should not be empty! ");

        var availableCurrencies = CurrencyService.GetCurrencies();
        if (availableCurrencies.Any(p => p.Id == ev.FeeCurrency) == false)
            throw new DomainException($"The fee currency {ev.FeeCurrency} is unknown!");
    }
}