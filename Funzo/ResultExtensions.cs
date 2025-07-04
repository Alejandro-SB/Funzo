﻿namespace Funzo;

/// <summary>
/// Extensions for result type
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts the result of a task to a <see cref="Option{T}"/>
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <param name="task">The task to extract the result from</param>
    /// <returns>A task wrapping the result</returns>
    public static Task<Option<TOk>> AsOk<TOk, TErr>(this Task<Result<TOk, TErr>> task)
    {
        return task.Then(t => t.AsOk());
    }

    /// <summary>
    /// Applies a predicate to the result of a task
    /// </summary>
    /// <typeparam name="TOk"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    /// <param name="task">The task to create the continuation on</param>
    /// <param name="ok">The action to execute if the result is Ok</param>
    /// <param name="fail">The action to execute if the result fails</param>
    /// <returns>A wrapper around the original task</returns>
    public static Task Match<TOk, TErr>(this Task<Result<TOk, TErr>> task, Action<TOk> ok, Action<TErr> fail)
    {
        return task.Then(t =>
        {
            t.Match(ok, fail);
            return Task.CompletedTask;
        });
    }
}
