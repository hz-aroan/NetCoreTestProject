namespace Infrastructure.SQL.Main;

public partial class BasketItem
{
    public int BasketItemId { get; set; }

    public int? BasketHeadId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual BasketHead BasketHead { get; set; }

    public virtual Product Product { get; set; }
}
