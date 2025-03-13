namespace Restaurant.RestApi;

public class MaitreD
{
    private readonly int _capacity;

    public MaitreD(IEnumerable<Table> tables)
        : this(tables.ToArray())
    {
    }

    public MaitreD(params Table[] tables)
    {
        ArgumentNullException.ThrowIfNull(tables);
        _capacity = tables.Sum(t => t.Seats);
    }

    public bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var reservedSeats = existingReservations.Sum(r => r.Quantity);
        return reservedSeats + candidate.Quantity <= _capacity;
    }
}
