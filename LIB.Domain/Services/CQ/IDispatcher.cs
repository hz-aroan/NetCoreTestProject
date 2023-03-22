namespace LIB.Domain.Services.CQ;

public interface IDispatcher
{
    void Execute(ICommandArg commandArg);

    TResponse Query<TResponse>(IQueryRequest<TResponse> queryReq);
}