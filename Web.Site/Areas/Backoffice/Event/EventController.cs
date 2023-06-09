﻿using LIB.Domain.Contracts;

using Microsoft.AspNetCore.Mvc;

using Web.Site.Areas.Backoffice.Event.Models;

namespace Web.Site.Areas.Backoffice.Event;

[Area("Backoffice")]
public class EventController : Controller
{
    private readonly EventFactory Factory;



    public EventController(ILogger<EventController> logger, IDispatcher dispatcher)
    {
        Factory = new EventFactory(this, logger, dispatcher);
    }



    public async Task<IActionResult> Index()
    {
        var model = await Factory.GetIndexModel();
        return View("Index", model);
    }



    public IActionResult AddEvent()
    {
        var model = Factory.GetAddEventModel();
        return PartialView("_AddNewEvent", model);
    }



    [HttpPost]
    public IActionResult DoAddEvent(EventForm form)
    {
        var model = Factory.ProcessAddEvent(form);
        return Redirect("Index");
    }
}