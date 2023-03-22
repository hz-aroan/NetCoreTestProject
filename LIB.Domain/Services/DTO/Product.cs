using Infrastructure.SQL.Main;

namespace LIB.Domain.Services.DTO;

public class Product
{
    public Int32 ProductId { get; set; }

    public String Name { get; set; }

    public Boolean IsAvailable { get; set; }

    public Fee Fee { get; set; }



    public Product()
    {
    }



    public Product(Infrastructure.SQL.Main.Product p, String sign)
    {
        ProductId = p.ProductId;
        Name = p.Name;
        IsAvailable = p.IsAvailable;
        Fee = new Fee {
            CurrencyId = p.FeeCurrency,
            Amount = (Double?)p.FeeAmount ?? 0,
            CurrencySign = sign
        };
    }
}