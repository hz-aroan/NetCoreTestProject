using LIB.Domain.Features.Events;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LIB.Domain.Exceptions;
using LIB.Domain.Features.Baskets;
using LIB.Domain.Services;
using Web.Site.Areas.Site.Home.Models;
using Web.Site.Areas.Site.Shared.Models;
using Web.Site.Areas.Site.Shopping.Models;
using Microsoft.AspNetCore.Http;
using LIB.Domain.Features.Products;
using LIB.Domain.Services.DTO;
using LIB.Domain.Contracts;

namespace Web.Site.Areas.Site.Shopping;

public class ShoppingFactory
{
    private readonly IDispatcher Dispatcher;

    private readonly ILogger Logger;

    private readonly Controller Parent;



    public ShoppingFactory(Controller parent, ILogger logger, IDispatcher dispatcher)
    {
        Parent = parent;
        Logger = logger;
        Dispatcher = dispatcher;
    }



    public IndexViewModel GetIndexModel()
    {
        var session = LoadSession();
        if (session.AlreadySelectedAnEvent == false)
            return null;

        var result = new IndexViewModel {
            SelectedEvent = session.SelectedEvent,
            Basket = Dispatcher.Send(new GetBasketQry(session.SelectedBasketUid)).Result
        };
        return result;
    }



    public void SelectEvent(Int32 eventId)
    {
        var ev = Dispatcher.Send(new GetAvailableEventQry(eventId)).Result;

        var sessionData = LoadSession();
        if (sessionData.SelectedEventId == ev.EventId)
            return;

        if (sessionData.AlreadySelectedAnEvent)
            throw new DomainException("You have already selected an event to attend. Please finish shopping before selecting a new one!");

        var cartUid = Dispatcher.Send(new CreateBasketCmd(ev.EventId)).Result;

        sessionData.SelectedEvent = ev;
        sessionData.SelectedBasketUid = cartUid;

        SaveSession(sessionData);
    }



    public Boolean AddProductToEvent(Int32 productId, Int32 qty)
    {
        var session = LoadSession();
        if (session.AlreadySelectedAnEvent == false)
            return false;
        Dispatcher.Send(new AddProductToBasketCmd(session.SelectedBasketUid, productId, qty));
        return true;
    }



    public ProductListViewModel GetProducts()
    {
        var session = LoadSession();
        var result = new ProductListViewModel {
            Products = Dispatcher.Send(new GetAllAvailableProductsQry()).Result
        };
        return result;
    }



    // ---------------



    private void SaveSession(SessionData sessionData)
    {
        var sessionStr = JsonSerializer.Serialize(sessionData);
        Parent.HttpContext.Session.SetString(SessionData.SESSION_KEY_NAME, sessionStr);
    }



    private SessionData LoadSession()
    {
        var session = Parent.HttpContext.Session.GetString(SessionData.SESSION_KEY_NAME);
        var result = String.IsNullOrEmpty(session) ? new SessionData() : JsonSerializer.Deserialize<SessionData>(session);
        return result;
    }
}