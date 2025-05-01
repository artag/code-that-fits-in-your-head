using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("schedule")]
public class ScheduleController : ControllerBase
{
    [HttpGet("{year}/{month}/{day}"), Authorize(Roles = "MaitreD")]
    public ActionResult Get(int year, int month, int day)
    {
        return new OkObjectResult(
            new CalendarDto
            {
                Year = year,
                Month = month,
                Day = day,
                Days = new[] { new DayDto() }
            });
    }
}
