namespace Funzo;

/// <summary>
/// Extensions for Result, Option and Unions that depend on Tasks
/// </summary>
public static class TaskExtensions
{
    internal static async Task<TOut> Then<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> func)
    {
        var result = await task.ConfigureAwait(false);

        return func(result);
    }

    /// <summary>
    /// Maps and flattens the current instance to another type
    /// </summary>
    /// <typeparam name="TOut">The type of the result</typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task">The task to map</param>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed and then flattened, if not, <see cref="Option{T}.None"/> will be returned</returns>
    public static Task<Result<TOut, TErr>> Map<TResult, TErr, TOut>(this Task<ResultBase<TResult, TErr>> task, Func<TOut> map)
        where TResult : ResultBase<TResult, TErr>, IResultBase<TResult, TErr>
        => task.Then(t => t.Map(map));
    /// <summary>
    /// Maps and flattens the current instance to another type
    /// </summary>
    /// <typeparam name="TOut">The type of the result</typeparam>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task">The task to map</param>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed and then flattened, if not, <see cref="Option{T}.None"/> will be returned</returns>
    public static Task<Result<TOut, TErr>> Map<TResult, TOk, TErr, TOut>(this Task<ResultBase<TResult, TOk, TErr>> task, Func<TOk, TOut> map)
        where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
        => task.Then(t => t.Map(map));

}
