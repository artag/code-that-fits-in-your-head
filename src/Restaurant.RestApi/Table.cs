using System.Diagnostics.CodeAnalysis;

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

    internal bool Fits(int quantity) =>
        quantity <= Seats;

    internal Table Reserve(Reservation reservation) =>
        _table.Accept(new ReserveVisitor(reservation));

    private interface ITable
    {
        int Seats { get; }
        T Accept<T>(ITableVisitor<T> visitor);
    }

    private interface ITableVisitor<T>
    {
        T VisitStandard(int seats, Reservation? reservation);
        T VisitCommunal(int seats, IReadOnlyCollection<Reservation> reservations);
    }

    private sealed class StandardTable : ITable
    {
        private readonly Reservation? _reservation;

        public StandardTable(int seats)
        {
            Seats = seats;
        }

        public StandardTable(int seats, Reservation reservation)
        {
            Seats = seats;
            _reservation = reservation;
        }

        public int Seats { get; }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitStandard(Seats, _reservation);
        }
    }

    private sealed class CommunalTable : ITable
    {
        private readonly int _seats;
        private readonly IReadOnlyCollection<Reservation> _reservations;

        public CommunalTable(
            int seats, params Reservation[] reservations)
        {
            _seats = seats;
            _reservations = reservations;
        }

        public int Seats => _seats - _reservations.Sum(r => r.Quantity);

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitCommunal(Seats, _reservations);
        }
    }

    private sealed class IsStandardVisitor : ITableVisitor<bool>
    {
        public bool VisitStandard(int seats, Reservation? reservation)
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
            var standard = new StandardTable(seats, _reservation);
            return new Table(standard);
        }
    }
}
