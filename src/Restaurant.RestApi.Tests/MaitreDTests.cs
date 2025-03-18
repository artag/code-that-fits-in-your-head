using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi.Tests;

public class MaitreDTests
{
    [Theory]
    [ClassData(typeof(AcceptTestCases))]
    public void Accept(int[] tableSeats, int[] reservedSeats)
    {
        var tables = tableSeats.Select(Table.Communal);
        var sut = new MaitreD(tables);
        var rs = reservedSeats.Select(Some.Reservation.WithQuantity);
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(rs, r);

        Assert.True(actual);
    }

    [Theory]
    [ClassData(typeof(RejectTestCases))]
    public void Reject(
        IEnumerable<Table> tables,
        IEnumerable<Reservation> reservations)
    {
        var sut = new MaitreD(tables);
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(reservations, r);

        Assert.False(actual);
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "This class is instantiated via Reflection.")]
    [SuppressMessage(
        "Performance",
        "CA1861: Avoid constant arrays as arguments")]
    private sealed class AcceptTestCases : TheoryData<IEnumerable<int>, IEnumerable<int>>
    {
        public AcceptTestCases()
        {
            Add(new[] { 12 }, Array.Empty<int>());
            Add(new[] { 8, 11 }, Array.Empty<int>());
            Add(new[] { 8, 11 }, new int[] { 2 });
        }
    }

    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes",
        Justification = "This class is instantiated via Reflection.")]
    private sealed class RejectTestCases : TheoryData<IEnumerable<Table>, IEnumerable<Reservation>>
    {
        public RejectTestCases()
        {
            Add(new[] { Table.Communal(6), Table.Communal(6) },
                Array.Empty<Reservation>());

            Add(new[] { Table.Standard(12) },
                new[] { Some.Reservation.WithQuantity(1) });
        }
    }
}
