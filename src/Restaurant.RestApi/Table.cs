namespace Restaurant.RestApi;

public record Table
{
    private Table(bool isStandard, int seats)
    {
        Seats = seats;
        IsStandard = isStandard;
        IsCommunal = !isStandard;
    }

    public static Table Standard(int seats) =>
        new Table(true, seats);

    public static Table Communal(int seats) =>
        new Table(false, seats);

    public int Seats { get; init; }
    public bool IsStandard { get; }
    public bool IsCommunal { get; }

    public Table WithSeats(int newSeats) =>
        this with { Seats = newSeats };

    internal bool Fits(int quantity) =>
        quantity <= Seats;

    internal Table Reserve(int seatsToReserve) =>
        WithSeats(Seats - seatsToReserve);
}
