namespace Restaurant.RestApi;

public interface ITableVisitor<out T>
{
    T VisitStandard(int seats, Reservation? reservation);
    T VisitCommunal(
        int seats,
        IReadOnlyCollection<Reservation> reservations);
}
