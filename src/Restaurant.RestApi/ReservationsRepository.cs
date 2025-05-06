namespace Restaurant.RestApi;

internal static class ReservationsRepository
{
    internal static Task<IReadOnlyCollection<Reservation>> ReadReservations(
        this IReservationsRepository repository,
        int restaurantId,
        DateTime date)
    {
        var min = date.Date;
        var max = min.AddDays(1).AddTicks(-1);
        return repository.ReadReservations(restaurantId, min, max);
    }
}
