using LIB.Domain.Exceptions;
using LIB.Domain.Features.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

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
        SuccessExpected(async () =>
        {
            ClearDatabase();

            await Dispatcher.Send(new AddEventCmd("first", "1st descr", 10, "eur", true));

            
            var allEvents = Dispatcher.Send(new GetAllAvailableEventsQry()).Result;
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

            Dispatcher.Send(new AddEventCmd("first", "1st descr", -3, "eur", true));
        });
    }




    [TestMethod]
    public void GetAllAvailableEventsQry_Set_QueryExecutes()
    {
        SuccessExpected( async () =>
        {
            ClearDatabase();
            await Dispatcher.Send(new AddEventCmd("first", "1st descr", 10, "eur", true));
            await Dispatcher.Send(new AddEventCmd("second", "2nd descr", 20, "usd", false));

            var items = await Dispatcher.Send(new GetAllAvailableEventsQry());

            Assert.AreEqual(1, items.Count );
        });
    }
}