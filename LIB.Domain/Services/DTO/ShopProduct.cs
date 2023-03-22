using Infrastructure.SQL.Main;

namespace LIB.Domain.Services.DTO;

public class ShopProduct : Product
{
    public Int32 Quantity { get; set; }

    
    public ShopProduct(Infrastructure.SQL.Main.Product p, String sign, Int32 qty)
        : base(p, sign)
    {
        Quantity = qty;
    }
}