namespace Restaurant.RestApi;

public class ReservationDto
{
    public string? At { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }

    internal bool IsValid
    {
        get =>
            DateTime.TryParse(At, out var _)
            && Email is not null
            && 0 < Quantity;
    }
}
