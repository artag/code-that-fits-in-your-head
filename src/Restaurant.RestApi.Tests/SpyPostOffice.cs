using System.Collections.ObjectModel;

namespace Restaurant.RestApi.Tests;

internal sealed class SpyPostOffice : Collection<Reservation>, IPostOffice
{
    public Task EmailReservationCreated(Reservation reservation)
    {
        Add(reservation);
        return Task.CompletedTask;
    }
}
