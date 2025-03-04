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

    public async Task<ActionResult> Post([FromBody] ReservationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var reservation = dto.Validate();
        if (reservation is null)
            return new BadRequestResult();

        var reservations = await _repository
            .ReadReservations(reservation.At)
            .ConfigureAwait(false);
        var reservedSeats = reservations.Sum(r => r.Quantity);
        if (10 < reservedSeats + reservation.Quantity)
            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);

        await _repository.Create(reservation).ConfigureAwait(false);

        return new NoContentResult();
    }
}
