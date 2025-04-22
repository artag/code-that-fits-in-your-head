namespace Restaurant.RestApi;

public static class DtoConversions
{
    public static ReservationDto ToDto(this Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        return new ReservationDto
        {
            Id = reservation.Id.ToString("N"),
            At = reservation.At.ToIso8601DateTimeString(),
            Email = reservation.Email.ToString(),
            Name = reservation.Name.ToString(),
            Quantity = reservation.Quantity
        };
    }
}
