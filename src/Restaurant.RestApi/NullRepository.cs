#pragma warning disable CA1812

namespace Restaurant.RestApi;

internal sealed class NullRepository : IReservationsRepository
{
    public Task Create(Reservation reservation)
    {
        return Task.CompletedTask;
    }
}

#pragma warning restore CA1812
