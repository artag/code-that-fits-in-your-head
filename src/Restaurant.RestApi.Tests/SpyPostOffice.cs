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

    public Task EmailReservationUpdating(Reservation reservation)
    {
        Add(new Observation(Event.Updating, reservation));
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
        Updating = 1,
        Updated = 2,
        Deleted = 3
    }

    internal sealed record Observation(Event Event, Reservation Reservation);
}
