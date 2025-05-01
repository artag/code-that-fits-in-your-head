using FsCheck;
using FsCheck.Xunit;

namespace Restaurant.RestApi.Tests;

public class MaitreDScheduleTests
{
    [Property]
    public Property Schedule()
    {
        return Prop.ForAll(
                (from rs in Gens.Reservations
                 from  m in Gens.MaitreD(rs)
                 select (m, rs)).ToArbitrary(),
            t => ScheduleImp(t.m, t.rs));
    }

    private static void ScheduleImp(
        MaitreD sut,
        Reservation[] reservations)
    {
        var actual = sut.Schedule(reservations);

        Assert.NotNull(actual);
        Assert.Equal(
            reservations.Select(r => r.At).Distinct().Count(),
            actual.Count());
#pragma warning disable RCS1077 // Optimize LINQ method call
        Assert.Equal(
            actual.Select(o => o.At).OrderBy(d => d),
            actual.Select(o => o.At));
#pragma warning restore RCS1077 // Optimize LINQ method call

        Assert.All(actual, o => AssertTables(sut.Tables, o.Value));
        Assert.All(
            actual,
            o => AssertRelevance(reservations, sut.SeatingDuration, o));
    }

    private static void AssertTables(
        IEnumerable<Table> expected,
        IEnumerable<Table> actual)
    {
        var exp = expected.ToArray();
        var act = actual.ToArray();
        Assert.Equal(
            exp.Length,
            act.Length);
        Assert.Equal(
            exp.Sum(t => t.Capacity),
            act.Sum(t => t.Capacity));
    }

    private static void AssertRelevance(
        IEnumerable<Reservation> reservations,
        TimeSpan seatingDuration,
        Occurrence<List<Table>> occurrence)
    {
        var seating = new Seating(seatingDuration, occurrence.At);
        var expected = reservations
            .Select(r => (new Seating(seatingDuration, r.At), r))
            .Where(t => seating.Overlaps(t.Item1))
            .Select(t => t.r)
            .ToHashSet();

        var actual = occurrence.Value
            .SelectMany(t => t.Accept(new ReservationsVisitor()))
            .ToHashSet();

        Assert.True(
            expected.SetEquals(actual),
            $"Expected: {expected}; actual {actual}.");
    }
}
