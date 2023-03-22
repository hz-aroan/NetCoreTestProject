using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Services.DTO;

public class Event
{
    public Int32 EventId { get; set; }

    public String Name { get; set; }

    public Boolean IsAvailable { get; set; }

    public String Description { get; set; }

    public Fee Fee { get; set; }



    public Event()
    {
    }



    internal Event(Infrastructure.SQL.Main.Event p, String currencySign)
    {
        EventId = p.EventId;
        Name = p.Name;
        Fee = new Fee {
            Amount = (Double?)p.FeeAmount ?? 0,
            CurrencyId = p.FeeCurrency,
            CurrencySign = currencySign
        };
        IsAvailable = p.IsAvailable;
        Description = p.Description ?? "";
    }
}