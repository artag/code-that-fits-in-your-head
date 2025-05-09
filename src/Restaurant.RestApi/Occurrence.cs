﻿using System.Collections;

namespace Restaurant.RestApi;

public sealed class Occurrence<T>
{
    public Occurrence(DateTime at, T value)
    {
        At = at;
        Value = value;
    }

    public DateTime At { get; }
    public T Value { get; }

    public Occurrence<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        return new Occurrence<TResult>(At, selector(Value));
    }

    public override bool Equals(object? obj)
    {
        return obj is Occurrence<T> occurrence &&
               At == occurrence.At &&
               ValueEquals(occurrence.Value);
    }

    private bool ValueEquals(T x)
    {
        if (Value is IStructuralEquatable seq)
            return seq.Equals(
                x,
                StructuralComparisons.StructuralEqualityComparer);

        return EqualityComparer<T>.Default.Equals(Value, x);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(At, Value);
    }
}

public static class Occurrence
{
    public static Occurrence<T> At<T>(this T value, DateTime at)
    {
        return new Occurrence<T>(at, value);
    }
}
