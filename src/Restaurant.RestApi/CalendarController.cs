using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    private readonly Table _table;

    public CalendarController(Table table)
    {
        _table = table;
    }

    [HttpGet("{year}")]
    public ActionResult Get(int year)
    {
        var daysInYear = new GregorianCalendar().GetDaysInYear(year);
        var firstDay = new DateTime(year, 1, 1);
        var days = Enumerable.Range(0, daysInYear)
            .Select(i => MakeDay(firstDay, i))
            .ToArray();
        return new OkObjectResult(
            new CalendarDto { Year = year, Days = days });
    }

    [HttpGet("{year}/{month}")]
    public ActionResult Get(int _, int __)
    {
        return new OkObjectResult(
            new CalendarDto
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month
            });
    }

    private DayDto MakeDay(DateTime origin, int days)
    {
        return new DayDto
        {
            Date = origin.AddDays(days)
                .ToString("o", CultureInfo.InvariantCulture),
            MaximumPartySize = _table.Seats
        };
    }
}
