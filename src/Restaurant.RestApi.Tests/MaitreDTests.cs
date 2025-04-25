using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi.Tests;

public class MaitreDTests
{
    [Theory]
    [ClassData(typeof(AcceptTestCases))]
    public void Accept(
        MaitreD sut,
        DateTime now,
        IEnumerable<Reservation> reservations)
    {
        ArgumentNullException.ThrowIfNull(sut);
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(now, reservations, r);

        Assert.True(actual);
    }

    [Theory]
    [ClassData(typeof(RejectTestCases))]
    public void Reject(
        MaitreD sut,
        DateTime now,
        IEnumerable<Reservation> reservations)
    {
        ArgumentNullException.ThrowIfNull(sut);
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(now, reservations, r);

        Assert.False(actual);
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "This class is instantiated via Reflection.")]
    [SuppressMessage(
        "Performance",
        "CA1861: Avoid constant arrays as arguments")]
    private sealed class AcceptTestCases
        : TheoryData<MaitreD, DateTime, IEnumerable<Reservation>>
    {
        public AcceptTestCases()
        {
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(12) }),
                Some.Now,
                Array.Empty<Reservation>());
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(8), Table.Communal(11) }),
                Some.Now,
                Array.Empty<Reservation>());
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(2), Table.Communal(11) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(2) });
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(11) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(11).TheDayBefore() });
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(11) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(11).TheDayAfter() });
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(2.5),
                new[] { Table.Standard(12) }),
                Some.Now,
                new [] { Some.Reservation
                    .WithQuantity(11)
                    .AddDate(TimeSpan.FromHours(-2.5)) });
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(1),
                new[] { Table.Standard(14) }),
                Some.Now,
                new[] { Some.Reservation
                    .WithQuantity(9)
                    .AddDate(TimeSpan.FromHours(1)) });
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "This class is instantiated via Reflection.")]
    private sealed class RejectTestCases
        : TheoryData<MaitreD, DateTime, IEnumerable<Reservation>>
    {
        public RejectTestCases()
        {
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Communal(6), Table.Communal(6) }),
                Some.Now,
                Array.Empty<Reservation>());
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Standard(12) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(1) });
            Add(new MaitreD(TimeSpan.FromHours(18),
                TimeSpan.FromHours(6),
                TimeSpan.FromHours(21),
                new[] { Table.Standard(11) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(1).OneHourBefore() });
            Add(new MaitreD(
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(21),
                TimeSpan.FromHours(6),
                new[] { Table.Standard(12) }),
                Some.Now,
                new[] { Some.Reservation.WithQuantity(2).OneHourLater() });
            /* Some.Reservation.At is the time of the 'hard-coded'
             * reservation in the test below. Adding 30 minutes to it means
             * that the restaurant opens 30 minutes later than the desired
             * reservation time, and therefore must be rejected. */
            Add(new MaitreD(
                    Some.Reservation.At.AddMinutes(30).TimeOfDay,
                    TimeSpan.FromHours(21),
                    TimeSpan.FromHours(6),
                    new[] { Table.Standard(12) }),
                Some.Now,
                Array.Empty<Reservation>());
            /* Some.Reservation.At is the time of the 'hard-coded'
             * reservation in the test below. Subtracting 30 minutes from
             * it means that the restaurant's last seating is 30 minutes
             * before the reservation time, and therefore the reservation
             * must be rejected. */
            Add(new MaitreD(
                    TimeSpan.FromHours(18),
                    Some.Reservation.At.AddMinutes(-30).TimeOfDay,
                    TimeSpan.FromHours(6),
                    new[] { Table.Standard(12) }),
                Some.Now,
                Array.Empty<Reservation>());
            Add(new MaitreD(
                    TimeSpan.FromHours(18),
                    TimeSpan.FromHours(21),
                    TimeSpan.FromHours(6),
                    Table.Standard(12)),
                Some.Now.AddDays(30),
                Array.Empty<Reservation>());
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "This class is instantiated via Reflection.")]
    private sealed class ScheduleTestCases :
        TheoryData<MaitreD, IEnumerable<Reservation>, IEnumerable<Occurrence<Table[]>>>
    {
        public ScheduleTestCases()
        {
            // No reservations, so no occurrences:
            Add(new MaitreD(
                    TimeSpan.FromHours(18),
                    TimeSpan.FromHours(21),
                    TimeSpan.FromHours(6),
                    Table.Communal(12)),
                Array.Empty<Reservation>(),
                Array.Empty<Occurrence<Table[]>>());
        }
    }

    [Theory, ClassData(typeof(ScheduleTestCases))]
    public void Schedule(
        MaitreD sut,
        IEnumerable<Reservation> reservations,
        IEnumerable<Occurrence<Table[]>> expected)
    {
        ArgumentNullException.ThrowIfNull(sut);
        var actual = sut.Schedule(reservations);
        Assert.Equal(
            expected.Select(o => o.Select(ts => ts.AsEnumerable())),
            actual);
    }
}
