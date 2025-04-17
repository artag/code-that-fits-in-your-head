using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private static readonly SemaphoreSlim Semaphore1 = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim Semaphore2 = new SemaphoreSlim(1, 1);

    private static bool _ensuredTables;
    private readonly IReservationsRepository _repository;
    private readonly IPostOffice _postOffice;
    private readonly IDateTimeService _dateTime;
    private readonly MaitreD _maitreD;

    public ReservationsController(
        IReservationsRepository repository,
        IPostOffice postOffice,
        IDateTimeService dateTime,
        MaitreD maitreD)
    {
        _repository = repository;
        _postOffice = postOffice;
        _dateTime = dateTime;
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

        var id = dto.ParseId() ?? Guid.NewGuid();
        var reservation = dto.Validate(id);
        if (reservation is null)
            return new BadRequestResult();

        await Semaphore1.WaitAsync().ConfigureAwait(false);

        try
        {
            var reservations = await _repository
                .ReadReservations(reservation.At)
                .ConfigureAwait(false);

            if (!_maitreD.WillAccept(_dateTime.Now, reservations, reservation))
                return NoTables500InternalServerError();

            await _repository.Create(reservation).ConfigureAwait(false);
            await _postOffice.EmailReservationCreated(reservation).ConfigureAwait(false);
        }
        finally
        {
            Semaphore1.Release();
        }

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

        return new OkObjectResult(r.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(
        string id, [FromBody] ReservationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!Guid.TryParse(id, out var rid))
            return new NotFoundResult();

        var res = dto.Validate(rid);
        if (res is null)
            return new BadRequestResult();

        await Semaphore2.WaitAsync().ConfigureAwait(false);

        try
        {
            var existing =
                await _repository.ReadReservation(rid).ConfigureAwait(false);
            if (existing is null)
                return new NotFoundResult();

            var reservations = await _repository
                .ReadReservations(res.At)
                .ConfigureAwait(false);
            reservations = reservations
                .Where(r => r.Id != res.Id)
                .ToList();
            if (!_maitreD.WillAccept(_dateTime.Now, reservations, res))
                return NoTables500InternalServerError();

            if (existing.Email != res.Email)
                await _postOffice
                    .EmailReservationUpdating(existing)
                    .ConfigureAwait(false);

            await _repository.Update(res).ConfigureAwait(false);
            await _postOffice.EmailReservationUpdated(res).ConfigureAwait(false);
        }
        finally
        {
            Semaphore2.Release();
        }

        return new OkObjectResult(res.ToDto());
    }

    [HttpDelete("{id}")]
    public async Task Delete(string id)
    {
        var parsed = Guid.TryParse(id, out var rid);
        if (!parsed)
            return;

        var r = await _repository
            .ReadReservation(rid)
            .ConfigureAwait(false);
        await _repository
            .Delete(rid)
            .ConfigureAwait(false);
        if (r is { })
            await _postOffice
                .EmailReservationDeleted(r)
                .ConfigureAwait(false);
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
            value: reservation.ToDto());
    }
}
