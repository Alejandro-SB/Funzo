using System.Diagnostics.CodeAnalysis;

namespace Funzo;
/// <summary>
/// Class that represents a Result
/// </summary>
/// <typeparam name="TOk">Type of the Ok value</typeparam>
/// <typeparam name="TErr">Type of the Error value</typeparam>
/// <typeparam name="TResult">Type of the result</typeparam>
public abstract class ResultBase<TResult, TOk, TErr> : IEquatable<ResultBase<TResult, TOk, TErr>>
    where TResult : ResultBase<TResult, TOk, TErr>, IResultBase<TResult, TOk, TErr>
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

    /// <summary>
    /// Executes an action if the result is OK
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public TResult Inspect(Action<TOk> action)
    {
        if (IsOk)
        {
            action(OkValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an async action if the result is OK
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public async Task<TResult> InspectAsync(Func<TOk, Task> action)
    {
        if (IsOk)
        {
            await action(OkValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an action if the result is an Error
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public TResult InspectErr(Action<TErr> action)
    {
        if (!IsOk)
        {
            action(ErrValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an async action if the result is an Error
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
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
    public Option<TOk> AsOk() => IsOk ? Option.Some(OkValue!) : Option<TOk>.None;

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

    /// <summary>
    /// Creates a builder to map exceptions into a <see cref="ResultBase{TResult, TOk, TErr}"/>
    /// </summary>
    /// <returns>A builder to map exceptions into a <see cref="ResultBase{TResult, TOk, TErr}"/></returns>
    public static IResultBuilder<TResult, TOk, TErr> For() => new ResultBuilder<TResult, TOk, TErr>();

    /// <inheritdoc />
    public bool Equals(ResultBase<TResult, TOk, TErr>? other)
    {
        return other is not null
            && other.IsOk == IsOk
            && (
                (other.IsOk && other.OkValue!.Equals(OkValue!))
                || (!other.IsOk && other.ErrValue!.Equals(ErrValue!))
            );
    }

    /// <inheritdoc />
    public override int GetHashCode() => IsOk ? 0 : ErrValue!.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ResultBase<TResult, TOk, TErr> result && result.Equals(this);
    }

    /// <inheritdoc />
    public static bool operator ==(ResultBase<TResult, TOk, TErr> lhs, ResultBase<TResult, TOk, TErr> rhs)
        => lhs.Equals(rhs);
    /// <inheritdoc />
    public static bool operator !=(ResultBase<TResult, TOk, TErr> lhs, ResultBase<TResult, TOk, TErr> rhs)
        => !(lhs == rhs);
}

/// <summary>
/// Class that represents a Result
/// </summary>
/// <typeparam name="TResult">The type of the result</typeparam>
/// <typeparam name="TErr">The type of the error</typeparam>
public abstract class ResultBase<TResult, TErr> : IEquatable<ResultBase<TResult, TErr>>
    where TResult : ResultBase<TResult, TErr>, IResultBase<TResult, TErr>
{
    private protected readonly bool IsOk;
    private protected readonly TErr? ErrValue;

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
    /// <typeparam name="TOut">The final result type</typeparam>
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
    /// Returns a new <see cref="Result{T, TErr}"/> with a new return value for the Ok state
    /// </summary>
    /// <typeparam name="T">The new Ok value</typeparam>
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
    /// Returns a new <see cref="Result{T, TErr}"/> with a new return value for the Ok state
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

    /// <summary>
    /// Executes an action if the result is OK
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public TResult Inspect(Action action)
    {
        if (IsOk)
        {
            action();
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an async action if the result is OK
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public async Task<TResult> InspectAsync(Func<Task> action)
    {
        if (IsOk)
        {
            await action();
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an action if the result is an Error
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
    public TResult InspectErr(Action<TErr> action)
    {
        if (!IsOk)
        {
            action(ErrValue!);
        }

        return (TResult)this;
    }

    /// <summary>
    /// Executes an async action if the result is an Error
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>The same result</returns>
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

    /// <summary>
    /// Creates a builder to map exceptions into a <see cref="ResultBase{TResult, TOk, TErr}"/>
    /// </summary>
    /// <returns>A builder to map exceptions into a <see cref="ResultBase{TResult, TOk, TErr}"/></returns>
    public static IResultBuilder<TResult, TErr> For() => new ResultBuilder<TResult, TErr>();

    /// <inheritdoc />
    public bool Equals(ResultBase<TResult, TErr>? other)
    {
        return other is not null
            && other.IsOk == IsOk
            && (
                other.IsOk
                || other.ErrValue!.Equals(ErrValue!)
            );
    }
    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ResultBase<TResult, TErr> result && result.Equals(this);
    }

    /// <inheritdoc />
    public override int GetHashCode() => IsOk ? 0 : ErrValue!.GetHashCode();

    /// <inheritdoc />
    public static bool operator ==(ResultBase<TResult, TErr> lhs, ResultBase<TResult, TErr> rhs)
        => lhs.Equals(rhs);
    /// <inheritdoc />
    public static bool operator !=(ResultBase<TResult, TErr> lhs, ResultBase<TResult, TErr> rhs)
        => !(lhs == rhs);
}