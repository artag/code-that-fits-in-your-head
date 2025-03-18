namespace Restaurant.RestApi;

public record Table
{
    private Table(TableType tableType, int seats)
    {
        TableType = tableType;
        Seats = seats;
    }

    public static Table Standard(int seats) =>
        new Table(TableType.Standard, seats);

    public static Table Communal(int seats) =>
        new Table(TableType.Communal, seats);

    public TableType TableType { get; }
    public int Seats { get; init; }

    public Table WithSeats(int newSeats) =>
        this with { Seats = newSeats };
}
