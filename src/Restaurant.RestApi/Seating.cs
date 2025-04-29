namespace Restaurant.RestApi;

public sealed record Seating
{
    public Seating(TimeSpan seatingDuration, DateTime at)
    {
        SeatingDuration = seatingDuration;
        At = at;
    }

    public TimeSpan SeatingDuration { get; }

    public DateTime At { get; }

    public DateTime Start =>
        At;

    public DateTime End =>
        Start + SeatingDuration;

    public bool Overlaps(Reservation other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var otherSeating = new Seating(SeatingDuration, other.At);
        return Start < otherSeating.End && otherSeating.Start < End;
    }

    public bool Overlaps(Seating otherSeating)
    {
        ArgumentNullException.ThrowIfNull(otherSeating);
        return Start < otherSeating.End && otherSeating.Start < End;
    }
}
