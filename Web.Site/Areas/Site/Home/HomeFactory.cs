using LIB.Domain.Features.Events;
using LIB.Domain.Services.CQ;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Web.Site.Areas.Site.Home.Models;

namespace Web.Site.Areas.Site.Home;

public class HomeFactory
{
    private readonly IDispatcher Dispatcher;

    private readonly ILogger Logger;

    private readonly Controller Parent;



    public HomeFactory(Controller parent, ILogger logger, IDispatcher dispatcher)
    {
        Parent = parent;
        Logger = logger;
        Dispatcher = dispatcher;
    }



    public EventSelectionViewModel GetIndexModel()
    {
        return new EventSelectionViewModel {
            Events = Dispatcher.Query(new GetAllAvailableEventsQry())
        };
    }
}