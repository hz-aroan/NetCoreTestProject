using MediatR;

namespace LIB.Domain.Contracts;

public interface ICommandArg : IRequest
{ }

public interface ICommandArg<TResponse> : IRequest<TResponse>
{ }


public interface ICommandHandler<TCommand>: IRequestHandler<TCommand>
    where TCommand : ICommandArg
{ }


public interface ICommandHandler<TCommand, TResponse>: IRequestHandler<TCommand, TResponse>
    where TCommand : ICommandArg<TResponse>
{ }


//

public interface IQueryRequest<TResponse> : IRequest<TResponse>
{ }

public interface IQueryHandler<TRequest, TResponse>: IRequestHandler<TRequest, TResponse>
    where TRequest : IQueryRequest<TResponse>
{ }


public interface IDispatcher:IMediator 
{ };