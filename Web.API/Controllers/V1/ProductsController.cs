using LIB.Domain.Contracts;
using LIB.Domain.Features.Products;
using LIB.Domain.Services.DTO;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using Web.API.Helper;

namespace Web.API.Controllers.V1;

[ApiController]
[Route("api/products/v1")]

public class ProductsController : Controller
{
    private ILogger<ProductsController> Logger;
    private IDispatcher Dispatcher;
    private IConfiguration Config;

    private RestApiExecutionHelper Safe;



    public ProductsController(ILogger<ProductsController> logger, IDispatcher dispatcher, IConfiguration config)
    {
        Logger = logger;
        this.Dispatcher = dispatcher;
        this.Config = config;
        Safe = new RestApiExecutionHelper(this, logger);
    }


    [HttpGet("GetAvailableProducts")]
    [SwaggerOperation(
        Summary = "Get all the available products and its descriptions",
        OperationId = "GetAvailableProducts")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IList<Product>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult GetAvailableProducts()
    {
        return Safe.Execute(() =>
        {
            var products = Dispatcher.Send(new GetAllAvailableProductsQry());
            return Ok(Json(products));
        });
    }



    [HttpPost("CreateProduct")]
    [SwaggerOperation(
        Summary = "Creates a new product in the system",
        Description = "CurrencyId can be 'eur', 'usd', 'huf'",
        OperationId = "CreateProduct")]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    
    public IActionResult CreateProduct(String productName, Double feeAmount, String currencyId)
    {
        return Safe.Execute(() =>
        {
            Dispatcher.Send(new AddProductCmd(productName, feeAmount, currencyId, true));
            return Ok();
        });
    }
}
