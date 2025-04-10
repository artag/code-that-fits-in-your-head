﻿using System.Collections.ObjectModel;

namespace Restaurant.RestApi.Tests;

internal sealed class FakeDatabase : Collection<Reservation>, IReservationsRepository
{
    public Task EnsureTables(CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task Create(
        Reservation reservation, CancellationToken ct = default)
    {
        Add(reservation);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Reservation>> ReadReservations(
        DateTime dateTime, CancellationToken ct = default)
    {
        var min = dateTime.Date;
        var max = min.AddDays(1).AddTicks(-1);
        var reservations = this
            .Where(r => min <= r.At && r.At <= max)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Reservation>>(reservations);
    }

    public Task<Reservation?> ReadReservation(
        Guid id, CancellationToken ct = default)
    {
        var reservation = this.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(reservation);
    }

    public async Task Update(
        Reservation reservation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        await Delete(reservation.Id, ct);
        await Create(reservation, ct);
    }

    public Task Delete(
        Guid id, CancellationToken ct = default)
    {
        var reservation = this.SingleOrDefault(r => r.Id == id);
        if (reservation is { })   // reservation != null
            Remove(reservation);

        return Task.CompletedTask;
    }
}
