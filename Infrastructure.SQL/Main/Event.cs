namespace Infrastructure.SQL.Main;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; }

    public string FeeCurrency { get; set; }

    public decimal FeeAmount { get; set; }

    public bool IsAvailable { get; set; }

    public string Description { get; set; }

    public virtual ICollection<BasketHead> BasketHeads { get; } = new List<BasketHead>();
}
