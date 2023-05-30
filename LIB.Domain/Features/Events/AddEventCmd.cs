using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Services;

namespace LIB.Domain.Features.Events;

public sealed record AddEventCmd(String EventName, String Description, Double FeeAmount, String Currency, Boolean IsAvailable) : ICommandArg;



public class AddEventCmdHandler : ICommandHandler<AddEventCmd>
{
    private readonly IEFWrapper EFWrapper;
    private readonly ICurrencyHandlingService CurrencyService;



    public AddEventCmdHandler(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
    }


    public Task Handle(AddEventCmd cmd, CancellationToken cancellationToken)
    {
        using var ctx = EFWrapper.GetContext();
        var ev = new Event {
            Name = cmd.EventName,
            IsAvailable = cmd.IsAvailable,
            Description = cmd.Description,
            FeeAmount = (Decimal)cmd.FeeAmount,
            FeeCurrency = cmd.Currency
        };

        var validator = new EventValidatingService(CurrencyService);
        validator.ValidateNewOne(ev, ctx);

        ctx.Events.Add(ev);
        EFWrapper.SafeFinish(ctx, "An error happened adding the new event!");
        return Task.CompletedTask;
    }
}