namespace Funzo;

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TOk">The type of the Ok value</typeparam>
/// <typeparam name="TErr">The type of the Error value</typeparam>
public interface IResult<TOk, TErr>
{
}

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TErr">The type of the Error value</typeparam>
public interface IResult<TErr>
{
}

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TResult">The type of the Result instance</typeparam>
/// <typeparam name="TOk">The type of the Ok value</typeparam>
/// <typeparam name="TErr">The type of the Error value</typeparam>
public interface IResultBase<TResult, TOk, TErr>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Creates an instance of <typeparamref name="TResult"/> in an Ok state
    /// </summary>
    /// <param name="ok">The value of the operation</param>
    /// <returns>An instance of <typeparamref name="TResult"/></returns>
    public static abstract TResult Ok(TOk ok);
    /// <summary>
    /// Creates an instance of <typeparamref name="TResult"/> in an Error state
    /// </summary>
    /// <param name="err">The value of the operation</param>
    /// <returns>An instance of <typeparamref name="TResult"/></returns>
    public static abstract TResult Err(TErr err);
#endif
}

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TResult">The type of the Result instance</typeparam>
/// <typeparam name="TErr">The type of the Error value</typeparam>
public interface IResultBase<TResult, TErr>
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Creates an instance of <typeparamref name="TResult"/> in an Ok state
    /// </summary>
    /// <returns>An instance of <typeparamref name="TResult"/></returns>
    public static abstract TResult Ok();
    /// <summary>
    /// Creates an instance of <typeparamref name="TResult"/> in an Error state
    /// </summary>
    /// <param name="err">The value of the operation</param>
    /// <returns>An instance of <typeparamref name="TResult"/></returns>
    public static abstract TResult Err(TErr err);
#endif
}
