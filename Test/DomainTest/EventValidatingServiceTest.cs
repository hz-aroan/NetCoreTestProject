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
using Event = Infrastructure.SQL.Main.Event;

namespace Test.DomainTest;

[TestClass]
public class EventValidatingServiceTest : RepoTestBase
{
    [TestInitialize]
    public void SetUp()
    {
        Init();
    }



    [TestMethod]
    public void CreateNewEvent_EmptyName_ExceptionIsThrown()
    {
        ExceptionExpected(typeof(DomainException), "should not be empty", () =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = ""
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_NegativeFeeValue_ExceptionIsThrown()
    {
        ExceptionExpected(typeof(DomainException), "should not be negative", () =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = "Any name",
                FeeAmount = -1
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_EmptyCurrencyId_ExceptionIsThrown()
    {
        ExceptionExpected(typeof(DomainException), "should not be empty", () =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = "Any name",
                FeeAmount = 10,
                FeeCurrency = ""
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_NullCurrencyId_ExceptionIsThrown()
    {
        ExceptionExpected(typeof(DomainException), "should not be empty", () =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = "Any name",
                FeeAmount = 10,
                FeeCurrency = null
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_UnknownCurrencyId_ExceptionIsThrown()
    {
        ExceptionExpected(typeof(DomainException), "currency {STRING} is unknown", () =>
        {
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = "Any name",
                FeeAmount = 10,
                FeeCurrency = "unknown"
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_SameNameExists_SaveHappened()
    {
        ExceptionExpected(typeof(DomainException), "already exists", () =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent1 = new Event {
                Name = "A good name",
                FeeAmount = 10, FeeCurrency = "usd"
            };
            ctx.Events.Add(newEvent1);
            ctx.SaveChanges();

            var service = new EventValidatingService(CurrencyHandlingService);

            var newEvent2 = new Event {
                Name = "A good name",
                FeeAmount = 5, FeeCurrency = "usd"
            };
            service.ValidateNewOne(newEvent2, ctx);
        });
    }



    [TestMethod]
    public void CreateNewEvent_AllFieldsAreSet_DropsNoException()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            var ctx = DbctxFactory.CreateDbContext();

            var newEvent = new Event {
                Name = "Any name",
                FeeAmount = 10,
                FeeCurrency = "usd"
            };
            var service = new EventValidatingService(CurrencyHandlingService);

            service.ValidateNewOne(newEvent, ctx);
        });
    }
}