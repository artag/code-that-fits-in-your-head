namespace Restaurant.RestApi;

internal sealed class Day : IPeriod
{
    private readonly int _year;
    private readonly int _month;
    private readonly int _day;

    public Day(int year, int month, int day)
    {
        _year = year;
        _month = month;
        _day = day;
    }

    public T Accept<T>(IPeriodVisitor<T> visitor)
    {
        return visitor.VisitDay(_year, _month, _day);
    }

    public override bool Equals(object? obj)
    {
        return obj is Day day
               && _month == day._month;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_month);
    }
}
