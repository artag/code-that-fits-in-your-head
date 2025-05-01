using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IReservationsRepository _repository;

    public ScheduleController(IReservationsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{year}/{month}/{day}"), Authorize(Roles = "MaitreD")]
    public ActionResult Get(int year, int month, int day)
    {
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
                        Date = new DateTime(year, month, day)
                            .ToIso8601DateString(),
                        Entries = Array.Empty<TimeDto>()
                    }
                }
            });
    }
}
