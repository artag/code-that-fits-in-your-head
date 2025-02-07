using System.Collections.ObjectModel;

namespace Restaurant.RestApi.Tests;

internal sealed class FakeDatabase : Collection<Reservation>, IReservationsRepository
{
    public Task Create(Reservation reservation, CancellationToken ct = default)
    {
        Add(reservation);
        return Task.CompletedTask;
    }
}
