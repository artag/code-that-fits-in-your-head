namespace Restaurant.RestApi.Tests;

internal static class ReservationEnvy
{
    /// <summary>
    /// This method is useful for testing, but seems misplaced in the
    /// production code. Why would the system want to change the ID of a
    /// reservation?
    /// If it turns out that there's a valid reason, then consider moving
    /// this function to the Reservation class.
    /// </summary>
    /// <param name="reservation">Reservation.</param>
    /// <param name="newId">New id.</param>
    public static Reservation WithId(
        this Reservation reservation, Guid newId)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        return reservation with { Id = newId };
    }

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

    public static Reservation OneHourLater(this Reservation reservation) =>
        reservation.AddDate(TimeSpan.FromHours(1));

    public static Reservation TheDayAfter(this Reservation reservation) =>
        reservation.AddDate(TimeSpan.FromDays(1));
}
