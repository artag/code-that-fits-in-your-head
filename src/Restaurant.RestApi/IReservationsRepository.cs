namespace Restaurant.RestApi;

public interface IReservationsRepository
{
    Task EnsureTables(CancellationToken ct = default);

    Task Create(
        Reservation reservation, CancellationToken ct = default);

    Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime dateTime, CancellationToken ct = default);

    Task<Reservation?> ReadReservation(
        Guid id, CancellationToken ct = default);
}
