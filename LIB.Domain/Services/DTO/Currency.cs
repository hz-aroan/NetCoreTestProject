using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Services.DTO;

public class Currency
{
    public String Id { get; set; }

    public String Text { get; set; }

    public String Sign { get; set; }



    public Currency(String text, String id, String sign)
    {
        Text = text;
        Id = id;
        Sign = sign;
    }
}