namespace Restaurant.RestApi;

public interface IReservationsRepository
{
    Task EnsureTables(
        CancellationToken ct = default);

    Task Create(
        int restaurantId, Reservation reservation, CancellationToken ct = default);

    Task<IReadOnlyCollection<Reservation>> ReadReservations(
        int restaurantId, DateTime min, DateTime max, CancellationToken ct = default);

    Task<Reservation?> ReadReservation(
        Guid id, CancellationToken ct = default);

    Task Update(
        Reservation reservation, CancellationToken ct = default);

    Task Delete(
        Guid id, CancellationToken ct = default);
}
