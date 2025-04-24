namespace Restaurant.RestApi;

internal sealed class Year : IPeriod
{
    private readonly int _year;

    public Year(int year)
    {
        _year = year;
    }

    public T Accept<T>(IPeriodVisitor<T> visitor)
    {
        return visitor.VisitYear(_year);
    }

    public override bool Equals(object? obj)
    {
        return obj is Year year
               && _year == year._year;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_year);
    }
}
