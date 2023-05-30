using LIB.Domain.Contracts;
using LIB.Domain.Services.DTO;

namespace LIB.Domain.Features.Baskets;

public sealed record GetBasketQry(Guid BasketUid) : IQueryRequest<Basket>;


public class GetAllProductsOfBasketQryHandler : IQueryHandler<GetBasketQry, Basket>
{
    private readonly IBasketHandlingService BasketHandler;


    public GetAllProductsOfBasketQryHandler(IBasketHandlingService basketHandler)
    {
        BasketHandler = basketHandler;
    }


    public Task<Basket> Handle(GetBasketQry request, CancellationToken cancellationToken)
    {
        var result = BasketHandler.GetBasket(request.BasketUid);
        return Task.FromResult(result);
    }
}