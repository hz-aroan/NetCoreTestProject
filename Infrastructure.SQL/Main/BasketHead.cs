namespace Infrastructure.SQL.Main;

public partial class BasketHead
{
    public int BasketHeadId { get; set; }

    public Guid Uid { get; set; }

    public int? EventId { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; } = new List<BasketItem>();

    public virtual Event Event { get; set; }
}
