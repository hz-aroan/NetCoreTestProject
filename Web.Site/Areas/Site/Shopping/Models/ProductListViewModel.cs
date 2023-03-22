using LIB.Domain.Services.DTO;

namespace Web.Site.Areas.Site.Shopping.Models;

public class ProductListViewModel
{
    public IList<Product> Products { get; set; }
}