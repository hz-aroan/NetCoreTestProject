using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Site.Areas.Backoffice.Event.Models;

public class EventSelectionViewModel
{
    public IList<LIB.Domain.Services.DTO.Event> Events { get; set; }
}