using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

/// <summary>
/// Home controller.
/// </summary>
[Route("")]
public class HomeController : ControllerBase
{
    private readonly bool _enableCalendar;

    public HomeController(CalendarFlag calendarFlag)
    {
        ArgumentNullException.ThrowIfNull(calendarFlag);
        _enableCalendar = calendarFlag.Enabled;
    }

    /// <summary>
    /// Get method.
    /// </summary>
    public IActionResult Get()
    {
        var links = new List<LinkDto>();
        links.Add(Url.LinkToReservations());
        if (_enableCalendar)
        {
            var now = DateTime.Now;
            links.Add(Url.LinkToYear(now.Year));
            links.Add(Url.LinkToMonth(now.Year, now.Month));
            links.Add(Url.LinkToDay(now.Year, now.Month, now.Day));
        }

        return Ok(new HomeDto { Links = links.ToArray() });
    }
}
