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

namespace Test.DomainTest;

[TestClass]
public class EventDispatcherTest : RepoTestBase
{
    [TestInitialize]
    public void SetUp()
    {
        Init();
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