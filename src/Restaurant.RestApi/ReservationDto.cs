namespace Restaurant.RestApi;

public class ReservationDto
{
    public string? Id { get; set; }
    public string? At { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }

    public static ReservationDto FromReservation(
        Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        return new ReservationDto
        {
            Id = reservation.Id.ToString("N"),
            At = reservation.At.ToString("o"),
            Email = reservation.Email.ToString(),
            Name = reservation.Name.ToString(),
            Quantity = reservation.Quantity
        };
    }

    public static explicit operator ReservationDto(
        Reservation reservation)
    {
        return FromReservation(reservation);
    }

    internal Guid? ParseId()
    {
        if (Guid.TryParse(Id, out var id))
            return id;
        return null;
    }

    internal Reservation? Validate(Guid id)
    {
        if (!DateTime.TryParse(At, out var d))
            return null;
        if (Email is null)
            return null;
        if (Quantity < 1)
            return null;

        return new Reservation(
            id,
            d,
            new Email(Email),
            new Name(Name ?? string.Empty),
            Quantity);
    }
}
