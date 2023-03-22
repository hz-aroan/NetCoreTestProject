namespace LIB.Domain.Services.DTO;

public class Fee
{
    public String CurrencyId { get; set; }

    public Double Amount { get; set; }

    public String CurrencySign { get; set; }



    public override String ToString()
    {
        return $"{Amount} {CurrencySign}";
    }
}