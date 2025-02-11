namespace Restaurant.RestApi;

public record Reservation
{
    public Reservation(
        DateTime at,
        string email,
        string name,
        int quantity)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "The value must be a positive (non-zero) number.");

        At = at;
        Email = email;
        Name = name;
        Quantity = quantity;
    }

    public DateTime At { get; init; }
    public string Email { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }
}
