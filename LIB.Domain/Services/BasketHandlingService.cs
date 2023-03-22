using Infrastructure.SQL.Main;
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

internal class BasketHandlingService
{
    private readonly CurrencyHandlingService CurrencyService;

    private readonly IDbContextFactory<MainDbContext> DbctxFactory;

    

    public BasketHandlingService(IDbContextFactory<MainDbContext> dbctxFactory)
    {
        DbctxFactory = dbctxFactory;
        CurrencyService = new CurrencyHandlingService();
    }



    public Basket GetBasket(Guid basketUid)
    {
        using var ctx = DbctxFactory.CreateDbContext();
        var basket = ctx.BasketHeads.Include(p => p.Event)
            .Include(p => p.BasketItems)
            .Include(p => p.BasketItems)
            .ThenInclude(p => p.Product)
            .FirstOrDefault(p => p.Uid == basketUid);
        if (basket == null)
            throw new DomainException($"Basket not found - id={basketUid}!");

        var result = new Basket();
        result.BasketId = basketUid;
        result.Products = basket.BasketItems.Select(CreateShopProduct)
            .ToList();
        result.Payments = CalculatePayments(result.Products, basket.Event);

        return result;
    }



    public Guid CreateNewBasket(Int32 eventId)
    {
        var result = Guid.NewGuid();

        using var ctx = DbctxFactory.CreateDbContext();
        var basket = new BasketHead {
            EventId = eventId,
            Uid = result
        };
        ctx.BasketHeads.Add(basket);
        ctx.SaveChanges();

        return result;
    }



    public void AddProductToBasket(Guid basketUid, Int32 productId, Int32 quantity)
    {
        using var ctx = DbctxFactory.CreateDbContext();

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

        try
        {
            ctx.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new DomainException("Error happened during modifying your basket!", ex);
        }
    }



    // -----------



    private IList<Payment> CalculatePayments(IList<ShopProduct> products, Event selectedEvent)
    {
        var result = new List<Payment>();
        foreach (var product in products)
        {
            var payment = result.FirstOrDefault(p => p.CurrencySign == product.Fee.CurrencySign);

            if (payment == null)
            {
                payment = new Payment {
                    CurrencyId = product.Fee.CurrencyId,
                    CurrencySign = product.Fee.CurrencySign,
                    Amount = 0
                };
                result.Add(payment);
            }

            payment.Amount += product.Fee.Amount * product.Quantity;

            if (payment.Amount == 0)
                result.Remove(payment);
        }

        if (result.Any() == false)
        {
            var payment = new Payment {
                CurrencyId = selectedEvent.FeeCurrency,
                Amount = (Double?)selectedEvent.FeeAmount ?? 0,
                CurrencySign = CurrencyService.GetSing(selectedEvent.FeeCurrency)
            };
            result.Add(payment);
        }

        return result;
    }



    private ShopProduct CreateShopProduct(BasketItem item)
    {
        var sign = CurrencyService.GetSing(item.Product.FeeCurrency);
        var result = new ShopProduct(item.Product, sign, item.Quantity);
        return result;
    }
}