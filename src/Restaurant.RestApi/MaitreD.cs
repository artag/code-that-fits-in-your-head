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

        var relevantReservations = existingReservations
            .Where(r => candidate.At <= r.At);

        var availableTables = Tables.ToList();
        foreach (var r in relevantReservations)
        {
            var table = availableTables.Find(t => r.Quantity <= t.Seats);
            if (table is { })
            {
                availableTables.Remove(table);
                if (table.IsCommunal)
                    availableTables.Add(table.Reserve(r.Quantity));
            }
        }

        return availableTables.Any(t => candidate.Quantity <= t.Seats);
    }
}
