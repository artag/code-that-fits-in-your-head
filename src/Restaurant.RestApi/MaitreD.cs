namespace Restaurant.RestApi;

internal static class MaitreD
{
    internal static bool WillAccept(
        IEnumerable<Reservation> existingReservations,
        Reservation candidate)
    {
        var reservedSeats = existingReservations.Sum(r => r.Quantity);
        return reservedSeats + candidate.Quantity <= 10;
    }
}
