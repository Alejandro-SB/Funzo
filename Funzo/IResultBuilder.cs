namespace Funzo;
public interface IResultBuilder<TResult, in TOk, in TErr>
{
    IResultBuilder<TResult, TOk, TErr> MapException<TException>(Func<TException, TErr> map)
        where TException : Exception;
    IResultBuilder<TResult, TOk, TErr> MapElse(Func<Exception, TErr> map);
    TResult Try(Func<TResult> action);
    Task<TResult> TryAsync(Func<Task<TResult>> action);
}

public interface IResultBuilder<TResult, in TErr>
{
    IResultBuilder<TResult, TErr> MapException<TException>(Func<TException, TErr> map)
        where TException : Exception;
    IResultBuilder<TResult, TErr> MapElse(Func<Exception, TErr> map);
    TResult Try(Func<TResult> action);
    Task<TResult> TryAsync(Func<Task<TResult>> action);
}
