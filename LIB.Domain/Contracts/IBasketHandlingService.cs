using LIB.Domain.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Contracts;

public interface IBasketHandlingService
{
    Basket GetBasket(Guid basketUid);

    Guid CreateNewBasket(Int32 eventId);

    void AddProductToBasket(Guid basketUid, Int32 productId, Int32 quantity);
}