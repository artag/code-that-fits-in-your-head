namespace Restaurant.RestApi;

public sealed class Table
{
    private readonly ITable _table;

    private Table(ITable table)
    {
        _table = table;
    }

    public static Table Standard(int seats)
    {
        return new Table(new StandardTable(seats));
    }

    public static Table Communal(int seats)
    {
        return new Table(new CommunalTable(seats));
    }

    public int Capacity =>
        Accept(new CapacityVisitor());

    internal bool Fits(int quantity)
    {
        var remainingSeats = Accept(new RemainingSeatsVisitor());
        return quantity <= remainingSeats;
    }

    public Table Reserve(Reservation reservation)
    {
        return Accept(new ReserveVisitor(reservation));
    }

    public T Accept<T>(ITableVisitor<T> visitor)
    {
        return _table.Accept(visitor);
    }

    public override bool Equals(object? obj)
    {
        return obj is Table table &&
               Equals(_table, table._table);
    }

    public override int GetHashCode()
    {
        return _table.GetHashCode();
    }

    private interface ITable
    {
        T Accept<T>(ITableVisitor<T> visitor);
    }

    private sealed class StandardTable : ITable
    {
        private readonly int _seats;
        private readonly Reservation? _reservation;

        public StandardTable(int seats)
        {
            _seats = seats;
        }

        public StandardTable(int seats, Reservation reservation)
        {
            _seats = seats;
            _reservation = reservation;
        }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitStandard(_seats, _reservation);
        }

        public override bool Equals(object? obj)
        {
            return obj is StandardTable table &&
                   _seats == table._seats &&
                   Equals(_reservation, table._reservation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_seats, _reservation);
        }
    }

    private sealed class CommunalTable : ITable
    {
        private readonly int _seats;
        private readonly IReadOnlyCollection<Reservation> _reservations;

        public CommunalTable(int seats, params Reservation[] reservations)
        {
            _seats = seats;
            _reservations = reservations;
        }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitCommunal(_seats, _reservations);
        }

        public override bool Equals(object? obj)
        {
            return obj is CommunalTable table &&
                   _seats == table._seats &&
                   _reservations.SequenceEqual(table._reservations);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_seats, _reservations);
        }
    }

    private sealed class ReserveVisitor : ITableVisitor<Table>
    {
        private readonly Reservation _reservation;

        public ReserveVisitor(Reservation reservation)
        {
            _reservation = reservation;
        }

        public Table VisitCommunal(
            int seats,
            IReadOnlyCollection<Reservation> reservations)
        {
            return new Table(
                new CommunalTable(
                    seats,
                    reservations.Append(_reservation).ToArray()));
        }

        public Table VisitStandard(int seats, Reservation? reservation)
        {
            return new Table(new StandardTable(seats, _reservation));
        }
    }

    private sealed class RemainingSeatsVisitor : ITableVisitor<int>
    {
        public int VisitCommunal(
            int seats,
            IReadOnlyCollection<Reservation> reservations)
        {
            return seats - reservations.Sum(r => r.Quantity);
        }

        public int VisitStandard(int seats, Reservation? reservation)
        {
            return reservation is null ? seats : 0;
        }
    }

    private sealed class CapacityVisitor : ITableVisitor<int>
    {
        public int VisitCommunal(
            int seats,
            IReadOnlyCollection<Reservation> reservations)
        {
            return seats;
        }

        public int VisitStandard(int seats, Reservation? reservation)
        {
            return seats;
        }
    }
}
