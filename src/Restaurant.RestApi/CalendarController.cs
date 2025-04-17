using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    [HttpGet("{year}")]
    public ActionResult Get(int year)
    {
        var daysInYear = new GregorianCalendar().GetDaysInYear(year);
        var days = Enumerable.Repeat(new DayDto(), daysInYear).ToArray();
        return new OkObjectResult(
            new CalendarDto { Year = year, Days = days });
    }
}
