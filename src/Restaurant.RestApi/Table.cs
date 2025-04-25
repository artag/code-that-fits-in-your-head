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

    internal bool Fits(int quantity)
    {
        int remainingSeats = _table.Accept(new RemainingSeatsVisitor());
        return quantity <= remainingSeats;
    }

    public Table Reserve(Reservation reservation) =>
        _table.Accept(new ReserveVisitor(reservation));

    private interface ITable
    {
        T Accept<T>(ITableVisitor<T> visitor);
    }

    private interface ITableVisitor<T>
    {
        T VisitStandard(int seats, Reservation? reservation);
        T VisitCommunal(int seats, IReadOnlyCollection<Reservation> reservations);
    }

    private sealed record StandardTable : ITable
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
    }

    private sealed record CommunalTable : ITable
    {
        private readonly int _seats;
        private readonly IReadOnlyCollection<Reservation> _reservations;

        public CommunalTable(
            int seats, params Reservation[] reservations)
        {
            _seats = seats;
            _reservations = reservations;
        }

        public T Accept<T>(ITableVisitor<T> visitor)
        {
            return visitor.VisitCommunal(_seats, _reservations);
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
}
