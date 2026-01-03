namespace Funzo;

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TOk">The type of the result if successful</typeparam>
/// <typeparam name="TErr">The type of the result if an error occurs</typeparam>
public sealed class Result<TOk, TErr> : ResultBase<Result<TOk, TErr>, TOk, TErr>, IEquatable<Result<TOk, TErr>>, IResultBase<Result<TOk, TErr>, TOk, TErr>
{
    private Result(TOk ok) : base(ok)
    {
    }

    private Result(TErr err) : base(err)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Result{TOk, TErr}"/> as a successful operation
    /// </summary>
    /// <param name="ok">The result of the successful operation</param>
    /// <returns>An instance of <see cref="Result{TOk, TErr}"/> as a successful operation</returns>
    public static Result<TOk, TErr> Ok(TOk ok) => new(ok);

    /// <summary>
    /// Creates a new instance of <see cref="Result{TOk, TErr}"/> as a failed operation
    /// </summary>
    /// <param name="err">The result of the failed operation</param>
    /// <returns>An instance of <see cref="Result{TOk, TErr}"/> as a failed operation</returns>
    public static Result<TOk, TErr> Err(TErr err) => new(err);

    /// <summary>
    /// Converts <typeparamref name="TOk"/> into <see cref="Result{TOk, TErr}" /> implicitly
    /// </summary>
    /// <param name="ok">Value</param>
    public static implicit operator Result<TOk, TErr>(TOk ok) => new(ok);
    /// <summary>
    /// Converts <typeparamref name="TErr"/> into <see cref="Result{TOk, TErr}" /> implicitly
    /// </summary>
    /// <param name="err">Value</param>
    public static implicit operator Result<TOk, TErr>(TErr err) => new(err);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Result<TOk, TErr> result && result.Equals(this);

    /// <inheritdoc />
    public bool Equals(Result<TOk, TErr>? other) 
        => other is not null
        && other.IsOk == IsOk
        && (
            IsOk && OkValue!.Equals(other.OkValue)
            || !IsOk && ErrValue!.Equals(other.ErrValue)
            );

    /// <inheritdoc />
    public override int GetHashCode() => IsOk ? OkValue!.GetHashCode() : ErrValue!.GetHashCode();
}

/// <inheritdoc />
public sealed class Result<TErr> : ResultBase<Result<TErr>, TErr>, IResultBase<Result<TErr>, TErr>
{
    /// <inheritdoc />
    private Result()
    {
    }

    /// <inheritdoc />
    private Result(TErr err) : base(err)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Result{TErr}"/> as a successful operation
    /// </summary>
    /// <returns>An instance of <see cref="Result{TErr}"/> as a successful operation</returns>
    public static Result<TErr> Ok() => new();
    /// <summary>
    /// Creates a new instance of <see cref="Result{TErr}"/> as a failed operation
    /// </summary>
    /// <param name="err">The result of the failed operation</param>
    /// <returns>An instance of <see cref="Result{TErr}"/> as a failed operation</returns>
    public static Result<TErr> Err(TErr err) => new(err);

    /// <summary>
    /// Converts <typeparamref name="TErr"/> into <see cref="Result{TErr}" /> implicitly
    /// </summary>
    /// <param name="err">Value</param>
    public static implicit operator Result<TErr>(TErr err) => new(err);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Result<TErr> result && result.Equals(this);

    /// <inheritdoc />
    public bool Equals(Result<TErr>? other) 
        => other is not null
        && other.IsOk == IsOk
        && (
            IsOk || ErrValue!.Equals(other.ErrValue)
            );

    /// <inheritdoc />
    public override int GetHashCode() => IsOk ? 0 : ErrValue!.GetHashCode();
}
