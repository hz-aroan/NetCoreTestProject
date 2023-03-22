using System.Security.Cryptography;
using Azure;

using LIB.Domain.Features.Baskets;
using LIB.Domain.Features.Events;
using LIB.Domain.Services.CQ;
using LIB.Domain.Services.DTO;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using Web.API.Helper;

namespace Web.API.Controllers.V1;

[ApiController]
[Route("api/events/v1")]

public class EventsController : Controller
{
    private ILogger<EventsController> Logger;
    private IDispatcher Dispatcher;
    private IConfiguration Config;

    private RestApiExecutionHelper Safe;



    public EventsController(ILogger<EventsController> logger, IDispatcher dispatcher, IConfiguration config)
    {
        Logger = logger;
        this.Dispatcher = dispatcher;
        this.Config = config;
        Safe = new RestApiExecutionHelper(this, logger);
    }


    [HttpPost("CreateEvent")]
    [SwaggerOperation(
        Summary = "Creates a new event",
        Description = "CurrencyId can be 'eur', 'usd', 'huf'",
        OperationId = "CreateEvent")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult CreateEvent(String eventName, String description, Double feeAmount, String currency)
    {
        return Safe.Execute(() =>
        {
            Dispatcher.Execute(new AddEventCmd(eventName, description, feeAmount, currency, true));
            return Ok();
        });
    }



    [HttpGet("GetAvailableEvents")]
    [SwaggerOperation(
        Summary = "Get all the available events and its descriptions",
        OperationId = "GetAvailableEvents")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IList<Event>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult GetAvailableEvents()
    {
        return Safe.Execute(() =>
        {
            var availableEvents = Dispatcher.Query(new GetAllAvailableEventsQry());
            return Ok(Json(availableEvents));
        });
    }



    [HttpPost("AttendToEvent")]
    [SwaggerOperation(
        Summary = "Attend to an event and creates a new basket for the event",
        Description = "Returns GUID of the new basket",
        OperationId = "AttendToEvent")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult AttendToEvent(Int32 eventId)
    {
        return Safe.Execute(() =>
        {
            var basketUid = Dispatcher.Query(new CreateBasketCmd(eventId));
            return Ok(Json(basketUid));
        });
    }
}
