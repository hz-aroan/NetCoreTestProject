using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Services.DTO;

public class Payment
{
    public Double Amount { get; set; }

    public String CurrencyId { get; set; }

    public String CurrencySign { get; set; }



    public override String ToString()
    {
        return $"{Amount} {CurrencySign}";
    }
}