using LIB.Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;

namespace Web.API.Helper;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class RestApiExecutionHelper
{
    private Controller Parent;

    private ILogger _logger;


    internal RestApiExecutionHelper(Controller parent, ILogger logger)
    {
        this.Parent = parent;
        _logger = logger;
    }



    internal IActionResult Execute(Func<IActionResult> sequence)
    {
        try
        {
            var result = sequence();
            return result;
        }
        catch (DomainException e)
        {
            _logger.LogTrace($"DomainException: {e.Message} of {e.ToString()}");
            return Parent.BadRequest(SimpleLine(e.Message));
        }
        catch (Exception e)
        {
            _logger.LogTrace($"General exception: {e.Message} of {e.ToString()}");
            var msg = "Something went wrong. Please contact to HEXAIO team for support.";
            var exc = e.ToString();
            return Parent.BadRequest(SimpleLine(msg + exc));
        }
    }



    // ---------------



    private String SimpleLine(String s)
    {
        return s.Replace("\r", "").Replace("\n", "");
    }
}
