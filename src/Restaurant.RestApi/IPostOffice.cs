namespace Restaurant.RestApi;

public interface IPostOffice
{
    Task EmailReservationCreated(Reservation reservation);

    Task EmailReservationDeleted(Reservation reservation);
}
