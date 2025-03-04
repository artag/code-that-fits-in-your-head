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
        if (!DateTime.TryParse(dto.At, out var d))
            return new BadRequestResult();
        if (!dto.IsValid)
            return new BadRequestResult();

        var reservations = await _repository.ReadReservations(d).ConfigureAwait(false);
        var reservedSeats = reservations.Sum(r => r.Quantity);
        if (10 < reservedSeats + dto.Quantity)
            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);

        var r = new Reservation(d, dto.Email!, dto.Name ?? string.Empty, dto.Quantity);
        await _repository.Create(r).ConfigureAwait(false);

        return new CreatedResult();
    }
}
