namespace LIB.Domain.Services.CQ;

public interface IQueryHandler<in TRequest, out TResponse>
    where TRequest : IQueryRequest<TResponse>
{
    TResponse Execute(TRequest queryArg);
}

public interface IQueryHandlerWithPermission<in TRequest, out TResponse> : IQueryHandler<TRequest, TResponse>
    where TRequest : IQueryRequest<TResponse>
{
}