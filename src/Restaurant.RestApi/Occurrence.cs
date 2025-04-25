namespace Restaurant.RestApi;

public sealed record Occurrence<T>
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
}

public static class Occurrence
{
    public static Occurrence<T> At<T>(this T value, DateTime at)
    {
        return new Occurrence<T>(at, value);
    }
}
