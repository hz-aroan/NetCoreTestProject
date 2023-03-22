using LIB.Domain.Features.Events;
using LIB.Domain.Services.CQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Site.Areas.Backoffice.Event.Models;

namespace Web.Site.Areas.Backoffice.Event;

public class EventFactory
{
    private readonly IDispatcher Dispatcher;

    private readonly ILogger Logger;

    private readonly Controller Parent;



    public EventFactory(Controller parent, ILogger logger, IDispatcher dispatcher)
    {
        Parent = parent;
        Logger = logger;
        Dispatcher = dispatcher;
    }



    public EventSelectionViewModel GetIndexModel()
    {
        var result = new EventSelectionViewModel {
            Events = Dispatcher.Query(new GetAllAvailableEventsQry())
        };
        return result;
    }



    public AddEventViewModel GetAddEventModel()
    {
        var availableCurrencies = Dispatcher.Query(new GetAvailableCurrenciesQry());
        var result = new AddEventViewModel {
            Currencies = availableCurrencies.Select(p => new SelectListItem { Value = p.Id, Text = p.Text })
                .ToList(),
            Currency = availableCurrencies.First()
                .Id
        };
        return result;
    }



    public Boolean ProcessAddEvent(EventForm form)
    {
        Dispatcher.Execute(new AddEventCmd(form.EventName, form.Description, form.FeeAmount, form.Currency, true));
        return true;
    }
}