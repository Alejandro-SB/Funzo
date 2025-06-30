using System.Diagnostics.CodeAnalysis;

namespace Funzo;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TErr"></typeparam>
public abstract class ResultBase<TResult, TOk, TErr> : IResult<TOk, TErr>
    where TResult : ResultBase<TResult, TOk, TErr>
{
    private protected readonly bool IsOk;
    private protected readonly TOk? OkValue;
    private protected readonly TErr? ErrValue;

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an OK result
    /// </summary>
    /// <param name="ok">The value for the OK type</param>
    protected ResultBase(TOk ok)
    {
        OkValue = ok;
        IsOk = true;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an Error result
    /// </summary>
    /// <param name="err">The value for the Error type</param>
    protected ResultBase(TErr err)
    {
        ErrValue = err;
        IsOk = false;
    }
    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    /// <returns>The result of the mapping function depending on the result</returns>
    public TOut Match<TOut>(Func<TOk, TOut> ok, Func<TErr, TOut> err) => IsOk ? ok(OkValue!) : err(ErrValue!);

    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    public TResult Match(Action<TOk> ok, Action<TErr> err)
    {
        if (IsOk)
        {
            ok(OkValue!);
        }
        else
        {
            err(ErrValue!);
        }

        return (TResult)this;
    }

    public TResult Inspect(Action<TOk> action)
    {
        if (IsOk)
        {
            action(OkValue!);
        }

        return (TResult)this;
    }

    public async Task<TResult> InspectAsync(Func<TOk, Task> action)
    {
        if (IsOk)
        {
            await action(OkValue!);
        }

        return (TResult)this;
    }

    public TResult InspectErr(Action<TErr> action)
    {
        if (!IsOk)
        {
            action(ErrValue!);
        }

        return (TResult)this;
    }

    public async Task<TResult> InspectErrAsync(Func<TErr, Task> action)
    {
        if (!IsOk)
        {
            await action(ErrValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Returns the <typeparamref name="TOk"/> value if result was ok or throws <see cref="ArgumentException"/>. This method should be avoided whenever possible in favour of <see cref="IsErr(out TErr?)"></see> or <see cref="IsErr(out TOk?, out TErr?)"></see>
    /// </summary>
    /// <returns>The <typeparamref name="TOk"/> value or throws</returns>
    /// <exception cref="ArgumentException"></exception>
    public TOk Unwrap()
    {
        return IsOk ? OkValue! : throw new ArgumentException("Result is in an error state");
    }

    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="err"/> when <see cref="Result{TOk, TErr}"/> is an Error, <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="err">The error contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsErr([NotNullWhen(true)] out TErr? err)
    {
        err = ErrValue;

        return !IsOk;
    }

    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="err"/> when <see cref="Result{TOk, TErr}"/> is an Error, <see langword="false"/> and assigns <paramref name="ok"/> otherwise.
    /// </summary>
    /// <param name="ok">The ok parameter contained in this instance, or <see langword="default"/> if no value present</param>
    /// <param name="err">The error contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsErr([NotNullWhen(false)] out TOk? ok, [NotNullWhen(true)] out TErr? err)
    {
        err = ErrValue;
        ok = OkValue;

        return !IsOk;
    }

    /// <summary>
    /// Converts the result into a <see cref="Option{TOk}"/>
    /// </summary>
    /// <returns><see cref="Option.Some{TOk}(TOk)" /> if the result is successful, <see cref="Option{TOk}.None"/> otherwise</returns>
    public Option<TOk> AsOk() => IsOk ? Option.Some(OkValue!) : Option<TOk>.None();

    /// <summary>
    /// Returns a new <see cref="Result{T, TErr}"/> with the value corresponding to <typeparamref name="TOk"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{T, TErr}"/> transformed</returns>
    public Result<T, TErr> Map<T>(Func<TOk, T> map)
    {
        if (IsOk)
        {
            return Result<T, TErr>.Ok(map(OkValue!));
        }

        return Result<T, TErr>.Err(ErrValue!);
    }

    /// <summary>
    /// Returns a new <see cref="Result{T, TErr}"/> with the value corresponding to <typeparamref name="TOk"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{T, TErr}"/> transformed</returns>
    public Result<T, TErr> Map<T>(Func<TOk, Result<T, TErr>> map)
    {
        if (IsOk)
        {
            return map(OkValue!);
        }

        return Result<T, TErr>.Err(ErrValue!);
    }

    /// <summary>
    /// Returns a new <see cref="Result{TOk, T}"/> with the value corresponding to <typeparamref name="TErr"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{TOk, T}"/> transformed</returns>
    public Result<TOk, T> MapErr<T>(Func<TErr, T> map)
    {
        if (IsOk)
        {
            return Result<TOk, T>.Ok(OkValue!);
        }

        return Result<TOk, T>.Err(map(ErrValue!));
    }

    /// <summary>
    /// Returns a new <see cref="Result{TOk, T}"/> with the value corresponding to <typeparamref name="TErr"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{TOk, T}"/> transformed</returns>
    public Result<TOk, T> MapErr<T>(Func<TErr, Result<TOk, T>> map)
    {
        if (IsOk)
        {
            return Result<TOk, T>.Ok(OkValue!);
        }

        return map(ErrValue!);
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TErr"></typeparam>
public abstract class ResultBase<TResult, TErr>
    where TResult : ResultBase<TResult, TErr>
{
    protected readonly bool IsOk;
    protected readonly TErr? ErrValue;

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an OK result
    /// </summary>
    protected ResultBase()
    {
        IsOk = true;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an Error result
    /// </summary>
    /// <param name="err">The value for the Error type</param>
    protected ResultBase(TErr err)
    {
        ErrValue = err;
        IsOk = false;
    }
    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <typeparam name="TResult">The final result type</typeparam>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    /// <returns>The result of the mapping function depending on the result</returns>
    public TOut Match<TOut>(Func<TOut> ok, Func<TErr, TOut> err) => IsOk ? ok() : err(ErrValue!);

    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    public TResult Match(Action ok, Action<TErr> err)
    {
        if (IsOk)
        {
            ok();
        }
        else
        {
            err(ErrValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Returns a new <see cref="Result{T, TErr}"/> with the value corresponding to <typeparamref name="TOk"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{T, TErr}"/> transformed</returns>
    public Result<T, TErr> Map<T>(Func<T> map)
    {
        if (IsOk)
        {
            return Result<T, TErr>.Ok(map());
        }

        return Result<T, TErr>.Err(ErrValue!);
    }

    /// <summary>
    /// Returns a new <see cref="Result{T, TErr}"/> with the value corresponding to <typeparamref name="TOk"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{T, TErr}"/> transformed</returns>
    public Result<T, TErr> Map<T>(Func<Result<T, TErr>> map)
    {
        if (IsOk)
        {
            return map();
        }

        return Result<T, TErr>.Err(ErrValue!);
    }

    /// <summary>
    /// Returns a new <see cref="Result{TOk, T}"/> with the value corresponding to <typeparamref name="TErr"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{TOk, T}"/> transformed</returns>
    public Result<T> MapErr<T>(Func<TErr, T> map)
    {
        if (IsOk)
        {
            return Result<T>.Ok();
        }

        return Result<T>.Err(map(ErrValue!));
    }

    /// <summary>
    /// Returns a new <see cref="Result{TOk, T}"/> with the value corresponding to <typeparamref name="TErr"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{TOk, T}"/> transformed</returns>
    public Result<T> MapErr<T>(Func<TErr, Result<T>> map)
    {
        if (IsOk)
        {
            return Result<T>.Ok();
        }

        return map(ErrValue!);
    }


    public TResult Inspect(Action action)
    {
        if (IsOk)
        {
            action();
        }

        return (TResult)this;
    }

    public async Task<TResult> InspectAsync(Func<Task> action)
    {
        if (IsOk)
        {
            await action();
        }

        return (TResult)this;
    }

    public TResult InspectErr(Action<TErr> action)
    {
        if (!IsOk)
        {
            action(ErrValue!);
        }

        return (TResult)this;
    }

    public async Task<TResult> InspectErrAsync(Func<TErr, Task> action)
    {
        if (!IsOk)
        {
            await action(ErrValue!);
        }

        return (TResult)this;
    }


    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="err"/> when <see cref="Result{TOk, TErr}"/> is an Error, <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="err">The error contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsErr([NotNullWhen(true)] out TErr? err)
    {
        err = ErrValue;

        return !IsOk;
    }
}