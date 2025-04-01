using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Restaurant.RestApi;

[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private static bool _ensuredTables;
    private readonly MaitreD _maitreD;
    private readonly IReservationsRepository _repository;

    public ReservationsController(
        IReservationsRepository repository,
        MaitreD maitreD)
    {
        _repository = repository;
        _maitreD = maitreD;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ReservationDto dto)
    {
        if (!_ensuredTables)
        {
            await _repository.EnsureTables().ConfigureAwait(false);
            _ensuredTables = true;
        }

        ArgumentNullException.ThrowIfNull(dto);

        var reservation = dto.Validate(Guid.NewGuid());
        if (reservation is null)
            return new BadRequestResult();

        var reservations = await _repository
            .ReadReservations(reservation.At)
            .ConfigureAwait(false);

        if (!_maitreD.WillAccept(DateTime.Now, reservations, reservation))
            return NoTables500InternalServerError();

        await _repository.Create(reservation).ConfigureAwait(false);

        return Reservation201Created(reservation);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
        if (!Guid.TryParse(id, out var rid))
            return new NotFoundResult();

        if (!_ensuredTables)
        {
            await _repository.EnsureTables().ConfigureAwait(false);
            _ensuredTables = true;
        }

        var r = await _repository
            .ReadReservation(rid)
            .ConfigureAwait(false);

        if (r is null)
            return new NotFoundResult();

        return new OkObjectResult(
            new ReservationDto
            {
                Id = id,
                At = r!.At.ToString("O"),
                Email = r.Email,
                Name = r.Name,
                Quantity = r.Quantity,
            });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(
        string id, [FromBody] ReservationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var r = dto.Validate(new Guid(id));
        if (r is null)
            return new BadRequestResult();

        await _repository.Update(r).ConfigureAwait(false);
        return new OkResult();
    }

    [HttpDelete("{id}")]
    public Task Delete(string id)
    {
        return Guid.TryParse(id, out var rid)
            ? _repository.Delete(rid)
            : Task.CompletedTask;
    }

    private static ObjectResult NoTables500InternalServerError()
    {
        return new ObjectResult("No tables available.")
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }

    private CreatedAtActionResult Reservation201Created(Reservation reservation)
    {
        return new CreatedAtActionResult(
            actionName: nameof(Get),
            controllerName: null,
            routeValues: new { id = reservation.Id.ToString("N") },
            value: null);
    }
}
