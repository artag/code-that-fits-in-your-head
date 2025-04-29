namespace Restaurant.RestApi;

public sealed record Seating
{
    public Seating(TimeSpan seatingDuration, Reservation reservation)
    {
        SeatingDuration = seatingDuration;
        Reservation = reservation;
    }

    public TimeSpan SeatingDuration { get; }

    public Reservation Reservation { get; }

    public DateTime Start =>
        Reservation.At;

    public DateTime End =>
        Start + SeatingDuration;

    public bool Overlaps(Reservation other)
    {
        var otherSeating = new Seating(SeatingDuration, other);
        return Start < otherSeating.End && otherSeating.Start < End;
    }
}
