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

    public Task EmailReservationDeleted(Reservation reservation)
    {
        Add(new Observation(Event.Deleted, reservation));
        return Task.CompletedTask;
    }

    public Task EmailReservationUpdated(Reservation reservation)
    {
        Add(new Observation(Event.Updated, reservation));
        return Task.CompletedTask;
    }

    internal enum Event
    {
        Created = 0,
        Updated = 1,
        Deleted = 2
    }

    internal sealed record Observation(Event Event, Reservation Reservation);
}
