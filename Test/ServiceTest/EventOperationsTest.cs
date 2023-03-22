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
using LIB.Domain.Features.Baskets;

namespace Test.DomainTest;

[TestClass]
public class EventOperationsTest : RepoTestBase
{
    [TestInitialize]
    public void SetUp()
    {
        Init();
    }

    
    [TestMethod]
    public void AddEvent_GoodEvent_EventIsStored()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();

            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));

            
            var allEvents = Dispatcher.Query(new GetAllAvailableEventsQry());
            var newEvent = allEvents.FirstOrDefault(p => p.Name == "first");

            Assert.IsNotNull(newEvent);
            Assert.AreEqual("1st descr", newEvent.Description);
            Assert.AreEqual(10, newEvent.Fee.Amount);
            Assert.AreEqual("eur", newEvent.Fee.CurrencyId);
            Assert.AreEqual("€", newEvent.Fee.CurrencySign);
        });
    }


    
    [TestMethod]
    public void AddEvent_NegativeServiceCost_ExceptionIsThrown()
    {
        ExceptionExpected( typeof(DomainException), "service fee should not be negative", () =>
        {
            ClearDatabase();

            Dispatcher.Execute(new AddEventCmd("first", "1st descr", -3, "eur", true));
        });
    }




    [TestMethod]
    public void GetAllAvailableEventsQry_Set_QueryExecutes()
    {
        SuccessExpected(() =>
        {
            ClearDatabase();
            Dispatcher.Execute(new AddEventCmd("first", "1st descr", 10, "eur", true));
            Dispatcher.Execute(new AddEventCmd("second", "2nd descr", 20, "usd", false));

            var items = Dispatcher.Query(new GetAllAvailableEventsQry());

            Assert.AreEqual(1, items.Count );
        });
    }
}