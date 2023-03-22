using System.Text.Json.Serialization;
using LIB.Domain.Services.DTO;

namespace Web.Site.Areas.Site.Shared.Models;

public class SessionData
{
    internal const String SESSION_KEY_NAME = "HexaIoSessionData";

    public Event SelectedEvent { get; set; }

    public Guid SelectedBasketUid { get; set; }



    [JsonIgnore] public Int32 SelectedEventId => SelectedEvent?.EventId ?? 0;

    [JsonIgnore] public Boolean AlreadySelectedAnEvent => SelectedEventId > 0;
}