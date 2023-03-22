using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Domain.Services.DTO;

public class Basket
{
    public IList<Payment> Payments { get; set; }

    public Guid BasketId { get; set; }

    public IList<ShopProduct> Products { get; set; }
}