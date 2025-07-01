﻿namespace Funzo;

/// <summary>
/// Represents an empty return of a function (<see langword="void"/>)
/// </summary>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    /// <summary>
    /// Default value for unit
    /// </summary>
    public static Unit Default => new();
    /// <inheritdoc/>
    public override int GetHashCode() => 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc/>
    public override string ToString() => "()";

    /// <inheritdoc/>
    public bool Equals(Unit other) => true;

    /// <inheritdoc/>
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc/>
    public static bool operator ==(Unit _, Unit __) => true;

    /// <inheritdoc/>
    public static bool operator !=(Unit _, Unit __) => false;

    /// <inheritdoc/>
    public static bool operator >(Unit _, Unit __) => false;

    /// <inheritdoc/>
    public static bool operator >=(Unit _, Unit __) => true;

    /// <inheritdoc/>
    public static bool operator <(Unit _, Unit __) => false;

    /// <inheritdoc/>
    public static bool operator <=(Unit _, Unit __) => true;

    /// <inheritdoc/>
    public static Unit operator +(Unit _, Unit __) => default;
}
