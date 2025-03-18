namespace Restaurant.RestApi;

public class MaitreD
{
    public MaitreD(params Table[] tables)
        : this(tables.AsEnumerable())
    {
    }

    public MaitreD(IEnumerable<Table> tables)
    {
        Tables = tables;
    }

    public IEnumerable<Table> Tables { get; }

    public bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(existingReservations);
        ArgumentNullException.ThrowIfNull(candidate);

        var relevantReservations =
            existingReservations.Where(candidate.Overlaps);

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
