namespace Restaurant.RestApi;

public sealed class ReservationsVisitor :
    ITableVisitor<IEnumerable<Reservation>>
{
    public IEnumerable<Reservation> VisitCommunal(
        int seats,
        IReadOnlyCollection<Reservation> reservations)
    {
        return reservations;
    }

    public IEnumerable<Reservation> VisitStandard(
        int seats,
        Reservation? reservation)
    {
        if (reservation is { })
            yield return reservation;
    }
}
