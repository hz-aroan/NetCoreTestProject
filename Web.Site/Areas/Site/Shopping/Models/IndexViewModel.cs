using LIB.Domain.Services.DTO;

namespace Web.Site.Areas.Site.Shopping.Models;

public class IndexViewModel
{
    public Event SelectedEvent { get; set; }

    public Basket Basket { get; set; }
}