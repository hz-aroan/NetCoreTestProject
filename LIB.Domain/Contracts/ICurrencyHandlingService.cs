using LIB.Domain.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Contracts;

public interface ICurrencyHandlingService
{
    IList<Currency> GetCurrencies();

    String GetSing(String currencyId);
}