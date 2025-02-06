using System.Collections.ObjectModel;

namespace Restaurant.RestApi.Tests;

internal sealed class FakeDatabase : Collection<Reservation>, IReservationsRepository
{
    public Task Create(Reservation reservation)
    {
        Add(reservation);
        return Task.CompletedTask;
    }
}
