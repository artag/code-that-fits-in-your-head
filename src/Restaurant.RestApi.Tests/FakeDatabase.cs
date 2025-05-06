using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi.Tests;

[SuppressMessage(
    "Naming",
    "CA1710:Identifiers should have correct suffix",
    Justification = "The role of the class is a Test Double.")]
public class FakeDatabase :
    ConcurrentDictionary<int, Collection<Reservation>>, IReservationsRepository
{
    public FakeDatabase()
    {
        Grandfather = new Collection<Reservation>();
        AddOrUpdate(RestApi.Grandfather.Id, Grandfather, (_, rs) => rs);
    }

    /// <summary>
    /// The 'original' restaurant 'grandfathered' in.
    /// </summary>
    /// <seealso cref="RestApi.Grandfather" />
    public Collection<Reservation> Grandfather { get; }

    public Task EnsureTables(CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task Create(
        int restaurantId,
        Reservation reservation,
        CancellationToken ct = default)
    {
        AddOrUpdate(
            restaurantId,
            new Collection<Reservation> { reservation },
            (_, rs) =>
            {
                rs.Add(reservation);
                return rs;
            });
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime min,
        DateTime max,
        CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyCollection<Reservation>>(
            GetOrAdd(RestApi.Grandfather.Id, new Collection<Reservation>())
                .Where(r => min <= r.At && r.At <= max).ToList());
    }

    public Task<Reservation?> ReadReservation(Guid id, CancellationToken ct = default)
    {
        var reservation =
            GetOrAdd(RestApi.Grandfather.Id, new Collection<Reservation>())
            .FirstOrDefault(r => r.Id == id);
        return Task.FromResult((Reservation?)reservation);
    }

    public async Task Update(Reservation reservation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        await Delete(reservation.Id, ct);
        await Create(RestApi.Grandfather.Id, reservation, ct);
    }

    public Task Delete(Guid id, CancellationToken ct = default)
    {
        var reservations =
            GetOrAdd(RestApi.Grandfather.Id, new Collection<Reservation>());
        var reservation = reservations.SingleOrDefault(r => r.Id == id);
        if (reservation is { })
            reservations.Remove(reservation);

        return Task.CompletedTask;
    }
}
