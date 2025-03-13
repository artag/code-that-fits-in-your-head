namespace Restaurant.RestApi;

public class Table
{
    public Table(TableType tableType, int seats)
    {
        TableType = tableType;
        Seats = seats;
    }

    public TableType TableType { get; }
    public int Seats { get; }
}
