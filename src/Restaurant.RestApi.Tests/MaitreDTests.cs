namespace Restaurant.RestApi.Tests;

public class MaitreDTests
{
    [Theory]
    [InlineData(new[] { 12 }, new int[0])]
    [InlineData(new[] { 8, 11 }, new int[0])]
    [InlineData(new[] { 8, 11 }, new int[] { 2 })]
    public void Accept(int[] tableSeats, int[] reservedSeats)
    {
        var tables = tableSeats.Select(s => new Table(TableType.Communal, s));
        var sut = new MaitreD(tables);
        var rs = reservedSeats
            .Select(s => new Reservation(
                new DateTime(2022, 4, 1, 20, 15, 0),
                "x@example.net",
                "",
                s));
        var r = Some.Reservation;

        var actual = sut.WillAccept(rs, r);

        Assert.True(actual);
    }

    [Fact]
    public void Reject()
    {
        var sut = new MaitreD(
            new Table(TableType.Communal, 6),
            new Table(TableType.Communal, 6));
        var r = Some.Reservation;

        var actual = sut.WillAccept(Array.Empty<Reservation>(), r);

        Assert.False(actual);
    }
}
