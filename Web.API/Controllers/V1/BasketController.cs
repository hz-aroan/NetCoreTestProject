using LIB.Domain.Contracts;
using LIB.Domain.Features.Baskets;
using LIB.Domain.Services.DTO;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using Web.API.Helper;

namespace Web.API.Controllers.V1;

[ApiController]
[Route("api/baskets/v1")]
public class BasketController: Controller
{
    private ILogger<EventsController> Logger;
    private IDispatcher Dispatcher;
    private IConfiguration Config;

    private RestApiExecutionHelper Safe;



    public BasketController(ILogger<EventsController> logger, IDispatcher dispatcher, IConfiguration config)
    {
        Logger = logger;
        this.Dispatcher = dispatcher;
        this.Config = config;
        Safe = new RestApiExecutionHelper(this, logger);
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
            Dispatcher.Send(new AddProductToBasketCmd(basketUid, productId, quantity));
            return Ok();
        });
    }



    [HttpGet("GetBasketQry")]
    [SwaggerOperation(
        Summary = "Queries a basket content",
        OperationId = "GetBasketQry")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Basket))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]

    public IActionResult GetBasketQry(Guid basketUid)
    {
        return Safe.Execute(() =>
        {
            var result = Dispatcher.Send(new GetBasketQry(basketUid));
            return Ok(Json(result));
        });
    }
}
