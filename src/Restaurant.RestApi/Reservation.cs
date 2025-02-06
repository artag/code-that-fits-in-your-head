namespace Restaurant.RestApi;

public record Reservation(
    DateTime At,
    string Email,
    string Name,
    int Quantity
);
