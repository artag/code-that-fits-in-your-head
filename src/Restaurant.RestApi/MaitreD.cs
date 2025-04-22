namespace Restaurant.RestApi;

public class MaitreD
{
    private readonly TimeOfDay _opensAt;
    private readonly TimeOfDay _lastSeating;
    private readonly TimeSpan _seatingDuration;

    public MaitreD(
        TimeOfDay opensAt,
        TimeOfDay lastSeating,
        TimeSpan seatingDuration,
        params Table[] tables)
        : this(opensAt, lastSeating, seatingDuration, tables.AsEnumerable())
    {
    }

    public MaitreD(
        TimeOfDay opensAt,
        TimeOfDay lastSeating,
        TimeSpan seatingDuration,
        IEnumerable<Table> tables)
    {
        _opensAt = opensAt;
        _lastSeating = lastSeating;
        _seatingDuration = seatingDuration;
        Tables = tables;
    }

    public IEnumerable<Table> Tables { get; }

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
        if (IsOutsideOfOpeningHours(candidate))
            return false;

        var seating = new Seating(_seatingDuration, candidate);
        var relevantReservations =
            existingReservations.Where(seating.Overlaps);
        var availableTables = Allocate(relevantReservations);
        return availableTables.Any(t => t.Fits(candidate.Quantity));
    }

    private bool IsOutsideOfOpeningHours(Reservation candidate)
    {
        return candidate.At.TimeOfDay < _opensAt
               || _lastSeating < candidate.At.TimeOfDay;
    }

    private List<Table> Allocate(
        IEnumerable<Reservation> reservations)
    {
        var availableTables = Tables.ToList();
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
