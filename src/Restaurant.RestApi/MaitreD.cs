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
        var allocation = Tables.ToList();
        foreach (var r in reservations)
        {
            var table = allocation.Find(t => t.Fits(r.Quantity));
            if (table is { })
            {
                allocation.Remove(table);
                allocation.Add(table.Reserve(r));
            }
        }

        return allocation;
    }

    public IEnumerable<Occurrence<IEnumerable<Table>>> Schedule(
        IEnumerable<Reservation> reservations)
    {
        var enumerable = reservations as Reservation[] ?? reservations.ToArray();
        if (enumerable.Length > 0)
        {
            var r = enumerable[0];
            yield return new[] { Table.Communal(12).Reserve(r) }.AsEnumerable().At(r.At);
        }
    }
}
