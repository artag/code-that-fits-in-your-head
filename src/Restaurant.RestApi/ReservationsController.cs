using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationsRepository _repository;

    public ReservationsController(
        IReservationsRepository repository)
    {
        _repository = repository;
    }

    public Task Post(ReservationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return _repository.Create(
            new Reservation(
                new DateTime(2023, 11, 24, 19, 0, 0),
                "juliad@example.net",
                "Julia Domna",
                5));
    }
}
