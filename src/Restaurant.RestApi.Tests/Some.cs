namespace Restaurant.RestApi.Tests;

public static class Some
{
    public static readonly DateTime Now =
        DateTime.Now.AddMinutes(5);

    public static readonly Reservation Reservation =
        new Reservation(
            Guid.NewGuid(),
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
