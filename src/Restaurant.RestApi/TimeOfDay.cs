namespace Restaurant.RestApi;

public readonly struct TimeOfDay : IEquatable<TimeOfDay>
{
    private readonly TimeSpan _durationSinceMidnight;

    public TimeOfDay(TimeSpan durationSinceMidnight)
    {
        if (durationSinceMidnight < TimeSpan.Zero
            || TimeSpan.FromHours(24) < durationSinceMidnight)
            throw new ArgumentOutOfRangeException(
                nameof(durationSinceMidnight),
                "Please supply a TimeSpan between 0 and 24 hours.");

        _durationSinceMidnight = durationSinceMidnight;
    }

    public static implicit operator TimeOfDay(TimeSpan timeSpan) =>
        new TimeOfDay(timeSpan);

    public static TimeOfDay ToTimeOfDay(TimeSpan timeSpan) =>
        new TimeOfDay(timeSpan);

    public bool Equals(TimeOfDay other)
    {
        return _durationSinceMidnight.Equals(other._durationSinceMidnight);
    }

    public override bool Equals(object? obj)
    {
        return obj is TimeOfDay other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _durationSinceMidnight.GetHashCode();
    }

    public static bool operator ==(TimeOfDay left, TimeOfDay right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TimeOfDay left, TimeOfDay right)
    {
        return !(left == right);
    }

    public static bool operator <(TimeOfDay left, TimeOfDay right)
    {
        return left._durationSinceMidnight < right._durationSinceMidnight;
    }

    public static bool operator >(TimeOfDay left, TimeOfDay right)
    {
        return left._durationSinceMidnight > right._durationSinceMidnight;
    }

    public int CompareTo(TimeOfDay other)
    {
        return _durationSinceMidnight.CompareTo(other._durationSinceMidnight);
    }
}
