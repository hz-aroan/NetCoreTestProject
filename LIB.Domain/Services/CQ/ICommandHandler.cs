namespace LIB.Domain.Services.CQ;

public interface ICommandHandler<in TCommandArg> where TCommandArg : ICommandArg
{
    void Execute(TCommandArg cmd);
}