namespace Restaurant.RestApi;

public class MaitreD
{
    public MaitreD(
        TimeSpan seatingDuration,
        params Table[] tables)
        : this(seatingDuration, tables.AsEnumerable())
    {
    }

    public MaitreD(
        TimeSpan seatingDuration,
        IEnumerable<Table> tables)
    {
        SeatingDuration = seatingDuration;
        Tables = tables;
    }

    public TimeSpan SeatingDuration { get; }
    public IEnumerable<Table> Tables { get; }

    public bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(existingReservations);
        ArgumentNullException.ThrowIfNull(candidate);

        var seating = new Seating(SeatingDuration, candidate);
        var relevantReservations =
            existingReservations.Where(seating.Overlaps);
        var availableTables = Allocate(relevantReservations);
        return availableTables.Any(t => t.Fits(candidate.Quantity));
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
