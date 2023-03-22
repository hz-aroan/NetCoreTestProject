using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Site.Areas.Backoffice.Product.Models;

public class ProductForm
{
    public Double FeeAmount { get; set; }

    public String Currency { get; set; }

    public String ProductName { get; set; }
}

public class AddProductViewModel : ProductForm
{
    public IList<SelectListItem> Currencies { get; set; }
}