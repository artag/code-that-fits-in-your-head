namespace Restaurant.RestApi;

internal sealed class Month : IPeriod
{
    private readonly int _year;
    private readonly int _month;

    public Month(int year, int month)
    {
        _year = year;
        _month = month;
    }

    public T Accept<T>(IPeriodVisitor<T> visitor)
    {
        return visitor.VisitMonth(_year, _month);
    }

    public override bool Equals(object? obj)
    {
        return obj is Month month
               && _year == month._year
               && _month == month._month;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_year, _month);
    }
}
