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



    [HttpPost("CreateBasket")]
    [SwaggerOperation(
        Summary = "Attend to an event and creates a new basket for the customer",
        Description = "Returns GUID of the new basket",
        OperationId = "CreateBasket")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult CreateBasket(Int32 eventId)
    {
        return Safe.Execute(() =>
        {
            var basketUid = Dispatcher.Query(new CreateBasketCmd(eventId));
            return Ok(Json(basketUid));
        });
    }



    [HttpPut("AddProductToBasketCmd")]
    [SwaggerOperation(
        Summary = "Add a product to a basket with the given quantity",
        OperationId = "AddProductToBasketCmd")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult AddProductToBasketCmd(Guid basketUid, Int32 productId, Int32 quantity)
    {
        return Safe.Execute(() =>
        {
            Dispatcher.Execute(new AddProductToBasketCmd(basketUid, productId, quantity));
            return Ok();
        });
    }
}
