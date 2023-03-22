using System.Diagnostics.Contracts;
using System.Reflection;

namespace LIB.Domain.Services.CQ;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _service;



    public Dispatcher(IServiceProvider service)
    {
        _service = service;
    }



    public void Execute(ICommandArg commandArg)
    {
        var requestType = commandArg.GetType();
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(requestType);
        var handler = _service.GetService(handlerType);
        Contract.Assume(handler != null);
        var methodName = nameof(ICommandHandler<ICommandArg>.Execute);
        var execMethod = handler.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new[] { requestType }, null);
        Contract.Assume(execMethod != null);
        try
        {
            execMethod.Invoke(handler, new[] { commandArg });
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }



    public TResponse Query<TResponse>(IQueryRequest<TResponse> queryReq)
    {
        var requestType = queryReq.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _service.GetService(handlerType);
        Contract.Assume(handler != null);
        var methodName = nameof(IQueryHandler<IQueryRequest<TResponse>, TResponse>.Execute);
        var execMethod = handler.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new[] { requestType }, null);
        Contract.Assume(execMethod != null);
        try
        {
            var result = execMethod.Invoke(handler, new[] { queryReq });
            return (TResponse)result;
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }
}