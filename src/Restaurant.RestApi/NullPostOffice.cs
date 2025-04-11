namespace Restaurant.RestApi;

public sealed class NullPostOffice : IPostOffice
{
    public static readonly NullPostOffice Instance = new NullPostOffice();

    private NullPostOffice()
    {
    }

    public Task EmailReservationCreated(Reservation reservation)
    {
        return Task.CompletedTask;
    }

    public Task EmailReservationDeleted(Reservation reservation)
    {
        return Task.CompletedTask;
    }
}
