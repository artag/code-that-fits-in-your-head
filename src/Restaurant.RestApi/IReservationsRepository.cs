namespace Restaurant.RestApi;

public interface IReservationsRepository
{
    Task Create(
        Reservation reservation, CancellationToken ct = default);

    Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime dateTime, CancellationToken ct = default);
}
