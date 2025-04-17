using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    [HttpGet("{year}")]
    public ActionResult Get(int year)
    {
        var days = Enumerable.Repeat(new DayDto(), 365).ToArray();
        return new OkObjectResult(
            new CalendarDto { Year = year, Days = days });
    }
}
