namespace Restaurant.RestApi;

public class MaitreD
{
    private readonly TimeSpan _opensAt;
    private readonly TimeSpan _lastSeating;
    private readonly TimeSpan _seatingDuration;
    private readonly IEnumerable<Table> _tables;

    public MaitreD(
        TimeSpan opensAt,
        TimeSpan lastSeating,
        TimeSpan seatingDuration,
        params Table[] tables)
        : this(opensAt, lastSeating, seatingDuration, tables.AsEnumerable())
    {
    }

    public MaitreD(
        TimeSpan opensAt,
        TimeSpan lastSeating,
        TimeSpan seatingDuration,
        IEnumerable<Table> tables)
    {
        _opensAt = opensAt;
        _lastSeating = lastSeating;
        _seatingDuration = seatingDuration;
        _tables = tables;
    }

    public bool WillAccept(
        DateTime now,
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(existingReservations);
        ArgumentNullException.ThrowIfNull(candidate);

        // Reject reservations in the past
        if (candidate.At < now)
            return false;

        // Reject reservation if it's outside of opening hours
        if (candidate.At.TimeOfDay < _opensAt ||
            _lastSeating < candidate.At.TimeOfDay)
            return false;

        var seating = new Seating(_seatingDuration, candidate);
        var relevantReservations =
            existingReservations.Where(seating.Overlaps);
        var availableTables = Allocate(relevantReservations);
        return availableTables.Any(t => t.Fits(candidate.Quantity));
    }

    private List<Table> Allocate(
        IEnumerable<Reservation> reservations)
    {
        var availableTables = _tables.ToList();
        foreach (var r in reservations)
        {
            var table = availableTables.Find(t => t.Fits(r.Quantity));
            if (table is { })
            {
                availableTables.Remove(table);
                if (table.IsCommunal)
                    availableTables.Add(table.Reserve(r.Quantity));
            }
        }

        return availableTables;
    }
}
