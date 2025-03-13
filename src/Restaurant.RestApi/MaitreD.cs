namespace Restaurant.RestApi;

public class MaitreD
{
    private readonly Table _table;

    public MaitreD(Table table)
    {
        _table = table ?? throw new ArgumentNullException(nameof(table));
    }

    public bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var reservedSeats = existingReservations.Sum(r => r.Quantity);
        return reservedSeats + candidate.Quantity <= _table.Seats;
    }
}
