namespace Restaurant.RestApi.Tests;

public static class Some
{
    public static DateTime Now => GetDummyNow();

    public static readonly Reservation Reservation =
        new Reservation(
            Guid.NewGuid(),
            Now,
            new Email("x@example.net"),
            new Name(""),
            1);

    public readonly static MaitreD MaitreD =
        new MaitreD(
            TimeSpan.FromHours(16),
            TimeSpan.FromHours(21),
            TimeSpan.FromHours(6),
            Table.Communal(10));

    private static DateTime GetDummyNow()
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, now.Month, now.Day, 20, 15, 0);
    }
}
