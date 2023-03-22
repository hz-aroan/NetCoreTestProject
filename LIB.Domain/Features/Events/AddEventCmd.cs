using Infrastructure.SQL.Main;
using LIB.Domain.Services.CQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LIB.Domain.Exceptions;
using LIB.Domain.Services;

namespace LIB.Domain.Features.Events;

public class AddEventCmd : ICommandArg
{
    internal readonly String Currency;

    internal readonly String Description;

    internal readonly String EventName;

    internal readonly Double FeeAmount;

    internal readonly Boolean IsAvailable;



    public AddEventCmd(String eventName, String description, Double feeAmount, String currency, Boolean isAvailable)
    {
        FeeAmount = feeAmount;
        Currency = currency;
        EventName = eventName;
        Description = description;
        IsAvailable = isAvailable;
    }
}

public class AddEventCmdHandler : ICommandHandler<AddEventCmd>
{
    private readonly IDbContextFactory<MainDbContext> DbctxFactory;



    public AddEventCmdHandler(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
    }



    public void Execute(AddEventCmd cmd)
    {
        var ev = new Event {
            Name = cmd.EventName,
            IsAvailable = cmd.IsAvailable,
            Description = cmd.Description,
            FeeAmount = (Decimal)cmd.FeeAmount,
            FeeCurrency = cmd.Currency
        };

        var availableCurrencies = CurrencyHandlingService.AvailableCurrencies;

        using var ctx = DbctxFactory.CreateDbContext();
        var validator = new EventValidatingService();
        validator.ValidateNewOne(ev, availableCurrencies, ctx);
        ctx.Events.Add(ev);

        try
        {
            ctx.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DomainException("An error happened adding the new event!", ex);
        }
    }
}