﻿using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("[controller]")]
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
            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);

        await _repository.Create(reservation).ConfigureAwait(false);

        return Reservation201Created(reservation);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
        if (!_ensuredTables)
        {
            await _repository.EnsureTables().ConfigureAwait(false);
            _ensuredTables = true;
        }

        var rid = new Guid(id);
        var r = await _repository
            .ReadReservation(rid)
            .ConfigureAwait(false);

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

    private CreatedAtActionResult Reservation201Created(Reservation reservation)
    {
        return new CreatedAtActionResult(
            actionName: nameof(Get),
            controllerName: null,
            routeValues: new { id = reservation.Id.ToString("N") },
            value: null);
    }
}
