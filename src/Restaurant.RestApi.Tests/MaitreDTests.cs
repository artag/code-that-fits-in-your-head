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
        var rs = reservedSeats.Select(Some.Reservation.WithQuantity);
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(rs, r);

        Assert.True(actual);
    }

    [Fact]
    public void Reject()
    {
        var sut = new MaitreD(
            new Table(TableType.Communal, 6),
            new Table(TableType.Communal, 6));
        var r = Some.Reservation.WithQuantity(11);

        var actual = sut.WillAccept(Array.Empty<Reservation>(), r);

        Assert.False(actual);
    }
}
