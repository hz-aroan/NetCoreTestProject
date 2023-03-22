using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Site.Areas.Backoffice.Event.Models;

public class EventForm
{
    public Double FeeAmount { get; set; }

    public String Currency { get; set; }

    public String EventName { get; set; }

    public String Description { get; set; }
}

public class AddEventViewModel : EventForm
{
    public IList<SelectListItem> Currencies { get; set; }
}