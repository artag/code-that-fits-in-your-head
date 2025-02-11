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
        if (dto.Email is null)
            return new BadRequestResult();
        if (dto.Quantity < 1)
            return new BadRequestResult();

        var r = new Reservation(d, dto.Email, dto.Name ?? string.Empty, dto.Quantity);
        await _repository.Create(r).ConfigureAwait(false);

        return new CreatedResult();
    }
}
