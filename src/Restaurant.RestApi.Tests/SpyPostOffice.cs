using System.Collections.ObjectModel;

namespace Restaurant.RestApi.Tests;

internal sealed class SpyPostOffice
    : Collection<SpyPostOffice.Observation>, IPostOffice
{
    public Task EmailReservationCreated(Reservation reservation)
    {
        Add(new Observation(Event.Created, reservation));
        return Task.CompletedTask;
    }

    internal enum Event
    {
        Created = 0
    }

    internal sealed record Observation(Event Event, Reservation Reservation);
}
