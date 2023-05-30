using LIB.Domain.Contracts;

namespace LIB.Domain.Features.Baskets;

public record AddProductToBasketCmd(Guid BasketUid, Int32 ProductId, Int32 Quantity) : ICommandArg;


public class AddProductToBasketCmdHandler : ICommandHandler<AddProductToBasketCmd>
{
    private readonly IBasketHandlingService BasketHandler;


    public AddProductToBasketCmdHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }

    public Task Handle(AddProductToBasketCmd queryArg, CancellationToken cancellationToken)
    {
        BasketHandler.AddProductToBasket(queryArg.BasketUid, queryArg.ProductId, queryArg.Quantity);
        return Task.CompletedTask;
    }
}