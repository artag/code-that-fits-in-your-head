namespace Restaurant.RestApi.Tests;

internal static class ReservationEnvy
{
    public static Reservation AddDate(
        this Reservation reservation,
        TimeSpan timeSpan)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        return reservation.WithDate(reservation.At.Add(timeSpan));
    }

    public static Reservation OneHourBefore(this Reservation reservation) =>
        reservation.AddDate(TimeSpan.FromHours(-1));

    public static Reservation TheDayBefore(this Reservation reservation) =>
        reservation.AddDate(TimeSpan.FromDays(-1));

    public static Reservation TheDayAfter(this Reservation reservation) =>
        reservation.AddDate(TimeSpan.FromDays(1));
}
