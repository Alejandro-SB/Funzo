namespace Funzo;


public interface IResult<TOk, TErr>
{
}
public interface IResult<TErr>
{
}

public interface IResultBase<TResult, TOk, TErr>
{
#if NET6_0_OR_GREATER
    public static abstract TResult Ok(TOk ok);
    public static abstract TResult Err(TErr ok);
#endif
}

public interface IResultBase<TResult, TErr>
{
#if NET6_0_OR_GREATER

    public static abstract TResult Ok();
    public static abstract TResult Err(TErr ok);
#endif
}
