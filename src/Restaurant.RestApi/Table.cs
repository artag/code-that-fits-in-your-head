namespace Restaurant.RestApi;

public record Table
{
    private readonly ITable _table;

    private Table(ITable table)
    {
        _table = table;
    }

    public static Table Standard(int seats) =>
        new Table(new StandardTable(seats));

    public static Table Communal(int seats) =>
        new Table(new CommunalTable(seats));

    public int Seats =>
        _table.Seats;

    public bool IsStandard =>
        _table.Accept(new IsStandardVisitor());

    public bool IsCommunal =>
        !IsStandard;

    public Table WithSeats(int newSeats) =>
        _table.Accept(new CopyAndUpdateSeatsVisitor(newSeats));

    internal bool Fits(int quantity) =>
        quantity <= Seats;

    internal Table Reserve(int seatsToReserve) =>
        WithSeats(Seats - seatsToReserve);

    private interface ITable
    {
        int Seats { get; }
        T Accept<T>(ITableVisitor<T> visitor);
    }

    private interface ITableVisitor<T>
    {
        T VisitStandard(int seats);
        T VisitCommunal(int seats);
    }

    private sealed class StandardTable : ITable
    {
        public StandardTable(int seats)
        {
            Seats = seats;
        }

        public int Seats { get; }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitStandard(Seats);
        }
    }

    private sealed class CommunalTable : ITable
    {
        public CommunalTable(int seats)
        {
            Seats = seats;
        }

        public int Seats { get; }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitCommunal(Seats);
        }
    }

    private sealed class CopyAndUpdateSeatsVisitor : ITableVisitor<Table>
    {
        private readonly int _newSeats;

        public CopyAndUpdateSeatsVisitor(int newSeats)
        {
            _newSeats = newSeats;
        }

        public Table VisitStandard(int seats)
        {
            return new Table(new StandardTable(_newSeats));
        }

        public Table VisitCommunal(int seats)
        {
            return new Table(new CommunalTable(_newSeats));
        }
    }

    private sealed class IsStandardVisitor : ITableVisitor<bool>
    {
        public bool VisitStandard(int seats)
        {
            return true;
        }

        public bool VisitCommunal(int seats)
        {
            return false;
        }
    }
}
