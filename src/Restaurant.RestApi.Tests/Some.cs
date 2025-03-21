namespace Restaurant.RestApi.Tests;

public static class Some
{
    public static readonly DateTime Now =
        new DateTime(2022, 4, 1, 20, 15, 0);

    public static readonly Reservation Reservation =
        new Reservation(
            Now,
            "x@example.net",
            "",
            1);

    public readonly static MaitreD MaitreD =
        new MaitreD(
            TimeSpan.FromHours(16),
            TimeSpan.FromHours(21),
            TimeSpan.FromHours(6),
            Table.Communal(10));
}
