using System.Globalization;
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

    public Task Post([FromBody] ReservationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var r = new Reservation(
            DateTime.Parse(dto.At!, CultureInfo.InvariantCulture),
            dto.Email!,
            dto.Name!,
            dto.Quantity);

        return _repository.Create(r);
    }
}
