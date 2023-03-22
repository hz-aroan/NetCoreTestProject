using LIB.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.DomainTest;

[TestClass]
public class CurrencyHandlingServiceTest
{
    [TestMethod]
    public void GetSign_USD_GoodSignReturned()
    {
        var service = new CurrencyHandlingService();
        var flag = service.GetSing("usd");
        Assert.AreEqual("$", flag);
    }



    [TestMethod]
    public void GetSign_USDCaseInsensitive_GoodSignReturned()
    {
        var service = new CurrencyHandlingService();
        var flag = service.GetSing("UsD");
        Assert.AreEqual("$", flag);
    }



    [TestMethod]
    public void GetSign_UknownCurrencyId_QuestionMarkReturned()
    {
        var service = new CurrencyHandlingService();
        var flag = service.GetSing("unknwon");
        Assert.AreEqual("?", flag);
    }
}