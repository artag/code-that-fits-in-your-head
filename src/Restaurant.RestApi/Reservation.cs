namespace Restaurant.RestApi;

public record Reservation
{
    public Reservation(
        Guid id,
        DateTime at,
        Email email,
        Name name,
        int quantity)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "The value must be a positive (non-zero) number.");

        Id = id;
        At = at;
        Email = email;
        Name = name;
        Quantity = quantity;
    }

    public Guid Id { get; init; }
    public DateTime At { get; init; }
    public Email Email { get; init; }
    public Name Name { get; init; }
    public int Quantity { get; init; }

    public Reservation WithDate(DateTime newAt) =>
        this with { At = newAt };

    public Reservation WithEmail(Email newEmail) =>
        this with { Email = newEmail };

    public Reservation WithName(Name newName) =>
        this with { Name = newName };

    public Reservation WithQuantity(int newQuantity) =>
        this with { Quantity = newQuantity };
}
