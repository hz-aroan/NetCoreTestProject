namespace Infrastructure.SQL.Main;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; }

    public bool IsAvailable { get; set; }

    public string FeeCurrency { get; set; }

    public decimal? FeeAmount { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; } = new List<BasketItem>();
}
