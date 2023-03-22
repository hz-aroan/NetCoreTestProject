using Infrastructure.SQL.Main;

using LIB.Domain.Contracts;
using LIB.Domain.Exceptions;
using LIB.Domain.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Event = Infrastructure.SQL.Main.Event;

namespace LIB.Domain.Services;

public class BasketHandlingService : IBasketHandlingService
{
    private readonly ICurrencyHandlingService CurrencyService;
    private readonly IEFWrapper EFWrapper;

    

    public BasketHandlingService(IEFWrapper efWrapper, ICurrencyHandlingService currencyService)
    {
        EFWrapper = efWrapper;
        CurrencyService = currencyService;
        
    }



    public Basket GetBasket(Guid basketUid)
    {
        using var ctx = EFWrapper.GetContext();
        var basket = ctx.BasketHeads.Include(p => p.Event)
            .Include(p => p.BasketItems)
            .ThenInclude(p => p.Product)
            .FirstOrDefault(p => p.Uid == basketUid);
        if (basket == null)
            throw new DomainException($"Basket not found - id={basketUid}!");

        var result = new Basket();
        result.BasketId = basketUid;
        result.Products = basket.BasketItems.Select(CreateShopProduct).ToList();
        result.Payments = CalculatePayments(result.Products, basket.Event);

        return result;
    }



    public Guid CreateNewBasket(Int32 eventId)
    {
        var result = Guid.NewGuid();

        using var ctx = EFWrapper.GetContext();
        var basket = new BasketHead {
            EventId = eventId,
            Uid = result
        };
        ctx.BasketHeads.Add(basket);
        EFWrapper.SafeFinish(ctx, "Error happened creating the basket!");

        return result;
    }



    public void AddProductToBasket(Guid basketUid, Int32 productId, Int32 quantity)
    {
        using var ctx = EFWrapper.GetContext();

        var basket = ctx.BasketHeads.Include(p => p.Event)
            .Include(p => p.BasketItems)
            .ThenInclude(p => p.Product)
            .FirstOrDefault(p => p.Uid == basketUid);
        if (basket == null)
            throw new DomainException($"Basket not found - uid={basketUid}!");

        var alreadyAddedProduct = basket.BasketItems.FirstOrDefault(p => p.ProductId == productId);
        if (alreadyAddedProduct != null)
        {
            alreadyAddedProduct.Quantity += quantity;
            if (alreadyAddedProduct.Quantity <= 0) basket.BasketItems.Remove(alreadyAddedProduct);
        }
        else
        {
            var product = ctx.Products.FirstOrDefault(p => p.IsAvailable && p.ProductId == productId);
            if (product == null)
                throw new DomainException($"Product - id={productId} - cannot be found or is not currently available!");

            var basketItem = new BasketItem {
                BasketHeadId = basket.BasketHeadId,
                ProductId = productId,
                Quantity = quantity
            };
            if (basketItem.Quantity > 0) basket.BasketItems.Add(basketItem);
        }
        EFWrapper.SafeFinish(ctx, "Error happened adding product to your basket!");
    }



    // -----------



    private IList<Payment> CalculatePayments(IList<ShopProduct> products, Event selectedEvent)
    {
        var result = CalcPayments(products);
        result = CleanUpPayments(result);

        if (result.Any() == false)
        {
            var defaultFee = CalcDefaultFee(selectedEvent);
            result.Add(defaultFee);
        }

        return result;
    }



    private List<Payment> CalcPayments(IList<ShopProduct> products)
    {
        var result = new List<Payment>();
        foreach (var product in products)
        {
            var payment = FindForCurrency(product.Fee, result);
            payment.Amount += product.Fee.Amount * product.Quantity;
        }
        return result;
    }


    private Payment CalcDefaultFee(Event selectedEvent)
    {
        var result = new Payment {
            CurrencyId = selectedEvent.FeeCurrency,
            Amount = (Double)selectedEvent.FeeAmount,
            CurrencySign = CurrencyService.GetSing(selectedEvent.FeeCurrency)
        };
        return result;
    }


    private List<Payment> CleanUpPayments(List<Payment> payments)
    {
        var result = payments.Where(p => p.Amount > 0);
        return result.ToList();
    }


    private Payment FindForCurrency(Fee fee, List<Payment> payments)
    {
        var payment = payments.FirstOrDefault(p => p.CurrencyId == fee.CurrencyId);

        if (payment == null)
        {
            payment = new Payment {
                CurrencyId = fee.CurrencyId,
                CurrencySign = fee.CurrencySign,
                Amount = 0
            };
            payments.Add(payment);
        }

        return payment;
    }


    private ShopProduct CreateShopProduct(BasketItem item)
    {
        var sign = CurrencyService.GetSing(item.Product.FeeCurrency);
        var result = new ShopProduct(item.Product, sign, item.Quantity);
        return result;
    }
}