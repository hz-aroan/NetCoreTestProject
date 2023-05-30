using LIB.Domain.Contracts;

namespace LIB.Domain.Features.Baskets;

public sealed record CreateBasketCmd(Int32 EventId) : ICommandArg<Guid>;



public class GetNewShoppingCartCmdHandler : ICommandHandler<CreateBasketCmd, Guid>
{
    private readonly IBasketHandlingService BasketHandler;



    public GetNewShoppingCartCmdHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }

    public Task<Guid> Handle(CreateBasketCmd request, CancellationToken cancellationToken)
    {
        var result = BasketHandler.CreateNewBasket(request.EventId);
        return Task.FromResult(result);
    }
}