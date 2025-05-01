using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IReservationsRepository _repository;
    private readonly MaitreD _maitreD;

    public ScheduleController(
        IReservationsRepository repository,
        MaitreD maitreD)
    {
        _repository = repository;
        _maitreD = maitreD;
    }

    [HttpGet("{year}/{month}/{day}"), Authorize(Roles = "MaitreD")]
    public async Task<ActionResult> Get(int year, int month, int day)
    {
        var date = new DateTime(year, month, day);
        var firstTick = date;
        var lastTick = firstTick.AddDays(1).AddTicks(-1);
        var reservations = await _repository
            .ReadReservations(firstTick, lastTick)
            .ConfigureAwait(false);
        var schedule = _maitreD.Schedule(reservations);
        var entries = schedule.Select(o => new TimeDto
        {
            Time = o.At.TimeOfDay.ToIso8601TimeString(),
            Reservations = o.Value
                .SelectMany(t => t.Accept(new ReservationsVisitor()))
                .Select(r => r.ToDto())
                .ToArray()
        }).ToArray();

        return new OkObjectResult(
            new CalendarDto
            {
                Year = year,
                Month = month,
                Day = day,
                Days = new[]
                {
                    new DayDto
                    {
                        Date = date.ToIso8601DateString(),
                        Entries = entries
                    }
                }
            });
    }

    private sealed class ReservationsVisitor :
        ITableVisitor<IEnumerable<Reservation>>
    {
        public IEnumerable<Reservation> VisitCommunal(
            int seats,
            IReadOnlyCollection<Reservation> reservations)
        {
            return reservations;
        }

        public IEnumerable<Reservation> VisitStandard(
            int seats,
            Reservation? reservation)
        {
            if (reservation is { })
                yield return reservation;
        }
    }
}
