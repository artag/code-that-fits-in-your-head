using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly MaitreD _maitreD;
    private readonly IReservationsRepository _repository;

    public ReservationsController(
        IReservationsRepository repository,
        MaitreD maitreD)
    {
        _repository = repository;
        _maitreD = maitreD;
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

        if (!_maitreD.WillAccept(DateTime.Now, reservations, reservation))
            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);

        await _repository.Create(reservation).ConfigureAwait(false);

        return new NoContentResult();
    }
}
