namespace Funzo;
public interface IResult<out TResult, in TOk, in TErr>
    where TResult : IResult<TResult, TOk, TErr>
{
#if NET6_0_OR_GREATER
    static abstract TResult Ok(TOk ok);
    static abstract TResult Err(TErr err);
#endif
}
