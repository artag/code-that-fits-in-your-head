namespace Restaurant.RestApi;

public class Table
{
    private readonly TableType _tableType;

    public Table(TableType tableType, int seats)
    {
        _tableType = tableType;
        Seats = seats;
    }

    public int Seats { get; }
}
