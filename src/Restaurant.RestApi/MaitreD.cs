namespace Restaurant.RestApi;

public class MaitreD
{
    private readonly TimeSpan _opensAt;
    private readonly TimeSpan _seatingDuration;
    private readonly IEnumerable<Table> _tables;

    public MaitreD(
        TimeSpan opensAt,
        TimeSpan seatingDuration,
        params Table[] tables)
        : this(opensAt, seatingDuration, tables.AsEnumerable())
    {
    }

    public MaitreD(
        TimeSpan opensAt,
        TimeSpan seatingDuration,
        IEnumerable<Table> tables)
    {
        _opensAt = opensAt;
        _seatingDuration = seatingDuration;
        _tables = tables;
    }

    public bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(existingReservations);
        ArgumentNullException.ThrowIfNull(candidate);
        if (candidate.At.TimeOfDay < _opensAt)
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
