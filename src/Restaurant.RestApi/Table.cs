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

    internal Table Reserve(Reservation reservation) =>
        WithSeats(Seats - reservation.Quantity);

    private interface ITable
    {
        int Seats { get; }
        T Accept<T>(ITableVisitor<T> visitor);
    }

    private interface ITableVisitor<T>
    {
        T VisitStandard(int seats);
        T VisitCommunal(int seats, IReadOnlyCollection<Reservation> reservations);
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
        private readonly IReadOnlyCollection<Reservation> _reservations;

        public CommunalTable(
            int seats, params Reservation[] reservations)
        {
            Seats = seats;
            _reservations = reservations;
        }

        public int Seats { get; }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitCommunal(Seats, _reservations);
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

        public Table VisitCommunal(
            int seats, IReadOnlyCollection<Reservation> reservations)
        {
            var communal = new CommunalTable(_newSeats, reservations.ToArray());
            return new Table(communal);
        }
    }

    private sealed class IsStandardVisitor : ITableVisitor<bool>
    {
        public bool VisitStandard(int seats)
        {
            return true;
        }

        public bool VisitCommunal(
            int seats, IReadOnlyCollection<Reservation> reservations)
        {
            return false;
        }
    }

    private sealed class ReserveVisitor : ITableVisitor<Table>
    {
        private readonly Reservation reservation;

        public ReserveVisitor(Reservation reservation)
        {
            this.reservation = reservation;
        }

        public Table VisitCommunal(
            int seats,
            IReadOnlyCollection<Reservation> reservations)
        {
            return new Table(
                new CommunalTable(
                    seats,
                    reservations.Append(reservation).ToArray()));
        }

        public Table VisitStandard(int seats)
        {
            return new Table(new StandardTable(seats));
        }
    }
}
