namespace Restaurant.RestApi;

public record Table
{
    public Table(TableType tableType, int seats)
    {
        TableType = tableType;
        Seats = seats;
    }

    public TableType TableType { get; }
    public int Seats { get; }
}
