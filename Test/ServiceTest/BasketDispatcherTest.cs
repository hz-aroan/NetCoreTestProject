using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIB.Domain.Services;
using Infrastructure.SQL.Main;
using LIB.Domain.Services.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LIB.Domain.Exceptions;
using LIB.Domain.Features.Events;
using LIB.Domain.Features.Products;
using LIB.Domain.Features.Baskets;
using Microsoft.VisualBasic.CompilerServices;

namespace Test.DomainTest;

[TestClass]
public class BasketDispatcherTest : RepoTestBase
{
    [TestInitialize]
    public void SetUp()
    {
        Init();
    }



    [TestMethod]
    public void GetBasket_EmptyBasket_PaymentEqualsToEventServiceFee()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));
            var anEvent = Dispatcher.Query(new GetAllAvailableEventsQry()).First();

            var basketUid = Dispatcher.Query(new CreateBasketCmd(anEvent.EventId));
            var basket = Dispatcher.Query(new GetBasketQry(basketUid));

            Assert.IsFalse(basket.Products.Any());

            Assert.AreEqual(1, basket.Payments.Count);
            var payment = basket.Payments[0];
            Assert.AreEqual(anEvent.Fee.Amount, payment.Amount);
            Assert.AreEqual(anEvent.Fee.CurrencyId, payment.CurrencyId);
            Assert.AreEqual(anEvent.Fee.CurrencySign, payment.CurrencySign);
        });
    }



    [TestMethod]
    public void GetBasket_OneProductInBasket_PaymentEqualsToProductFee()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 1", 1, "usd", true));
            Dispatcher.Execute(new AddProductCmd("product 2", 2, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 3", 3, "usd", true));
            var anEvent = Dispatcher.Query(new GetAllAvailableEventsQry()).First();
            var products = Dispatcher.Query(new GetAllAvailableProductsQry());
            var productA = products.First();

            var basketUid = Dispatcher.Query(new CreateBasketCmd(anEvent.EventId));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, 2));
            var basket = Dispatcher.Query(new GetBasketQry(basketUid));

            Assert.IsTrue(basket.Products.Any());
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(productA.ProductId, basket.Products.First()
                .ProductId);
            Assert.AreEqual(2, basket.Products.First()
                .Quantity);

            Assert.AreEqual(1, basket.Payments.Count);

            var payment = basket.Payments[0];
            Assert.AreEqual(productA.Fee.Amount * 2, payment.Amount);
            Assert.AreEqual(productA.Fee.CurrencyId, payment.CurrencyId);
            Assert.AreEqual(productA.Fee.CurrencySign, payment.CurrencySign);
        });
    }



    [TestMethod]
    public void GetBasket_AddRemoveProductsInBasket_PaymentCalculatedWell()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 1", 1, "usd", true));
            Dispatcher.Execute(new AddProductCmd("product 2", 2, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 3", 3, "usd", true));
            var anEvent = Dispatcher.Query(new GetAllAvailableEventsQry()).First();
            var products = Dispatcher.Query(new GetAllAvailableProductsQry());
            var productA = products.First();
            var productB = products.Skip(1).First();

            var basketUid = Dispatcher.Query(new CreateBasketCmd(anEvent.EventId));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, 2));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productB.ProductId, 3));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, -1));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, -1));
            var basket = Dispatcher.Query(new GetBasketQry(basketUid));

            Assert.IsTrue(basket.Products.Any());
            Assert.AreEqual(1, basket.Products.Count);
            Assert.AreEqual(productB.ProductId, basket.Products.First().ProductId);
            Assert.AreEqual(3, basket.Products.First().Quantity);

            Assert.AreEqual(1, basket.Payments.Count);

            var payment = basket.Payments[0];
            Assert.AreEqual(productB.Fee.Amount * 3, payment.Amount);
            Assert.AreEqual(productB.Fee.CurrencyId, payment.CurrencyId);
            Assert.AreEqual(productB.Fee.CurrencySign, payment.CurrencySign);
        });
    }


    
    [TestMethod]
    public void GetBasket_AddRemoveProductsButRemoveAllAtEnd_PaymentCalculatedServiceFee()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 1", 1, "usd", true));
            Dispatcher.Execute(new AddProductCmd("product 2", 2, "eur", true));
            Dispatcher.Execute(new AddProductCmd("product 3", 3, "usd", true));
            var anEvent = Dispatcher.Query(new GetAllAvailableEventsQry()).First();
            var products = Dispatcher.Query(new GetAllAvailableProductsQry());
            var productA = products.First();
            var productB = products.Skip(1).First();

            var basketUid = Dispatcher.Query(new CreateBasketCmd(anEvent.EventId));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, 1));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productB.ProductId, 3));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productA.ProductId, -1));
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productB.ProductId, -4));
            var basket = Dispatcher.Query(new GetBasketQry(basketUid));

            Assert.IsFalse(basket.Products.Any());
            Assert.AreEqual(1, basket.Payments.Count);

            var payment = basket.Payments[0];
            Assert.AreEqual(anEvent.Fee.Amount, payment.Amount);
            Assert.AreEqual(anEvent.Fee.CurrencyId, payment.CurrencyId);
            Assert.AreEqual(anEvent.Fee.CurrencySign, payment.CurrencySign);
        });
    }
}