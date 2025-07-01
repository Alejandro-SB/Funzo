namespace Funzo;

/// <summary>
/// Represents a builder that can transform <see cref="Exception"/> into a <typeparamref name="TResult"/>
/// </summary>
/// <typeparam name="TResult">The result to return</typeparam>
/// <typeparam name="TOk">The Ok parameter type</typeparam>
/// <typeparam name="TErr">The Error parameter type</typeparam>
public interface IResultBuilder<TResult, in TOk, in TErr>
{
    /// <summary>
    /// Maps the exception <typeparamref name="TException"/> to a <typeparamref name="TErr"/>
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="map">The mapping function</param>
    /// <returns>The same instance</returns>
    IResultBuilder<TResult, TOk, TErr> MapException<TException>(Func<TException, TErr> map)
        where TException : Exception;
    /// <summary>
    /// Maps all the other exceptions thrown
    /// </summary>
    /// <param name="map">The mapping function</param>
    /// <returns>The same instance</returns>
    IResultBuilder<TResult, TOk, TErr> MapElse(Func<Exception, TErr> map);
    /// <summary>
    /// Executes the action <paramref name="action"/> supplied
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>The result of the operation</returns>
    TResult Try(Func<TResult> action);
    /// <summary>
    /// Executes the asynchronous action <paramref name="action"/> supplied
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>The result of the operation</returns>
    Task<TResult> TryAsync(Func<Task<TResult>> action);
}

/// <summary>
/// Represents a builder that can transform <see cref="Exception"/> into a <typeparamref name="TResult"/>
/// </summary>
/// <typeparam name="TResult">The result to return</typeparam>
/// <typeparam name="TErr">The Error parameter type</typeparam>
public interface IResultBuilder<TResult, in TErr>
{
    /// <summary>
    /// Maps the exception <typeparamref name="TException"/> to a <typeparamref name="TErr"/>
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="map">The mapping function</param>
    /// <returns></returns>
    IResultBuilder<TResult, TErr> MapException<TException>(Func<TException, TErr> map)
        where TException : Exception;
    /// <summary>
    /// Maps all the other exceptions thrown
    /// </summary>
    /// <param name="map">The mapping function</param>
    /// <returns>The same instance</returns>
    IResultBuilder<TResult, TErr> MapElse(Func<Exception, TErr> map);
    /// <summary>
    /// Executes the action <paramref name="action"/> supplied
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>The result of the operation</returns>
    TResult Try(Func<TResult> action);
    /// <summary>
    /// Executes the asynchronous action <paramref name="action"/> supplied
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>The result of the operation</returns>
    Task<TResult> TryAsync(Func<Task<TResult>> action);
}
