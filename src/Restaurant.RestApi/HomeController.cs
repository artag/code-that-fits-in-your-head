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
            links.Add(
                Url.LinkToYear(
                    DateTime.Now.Year));
            links.Add(
                Url.LinkToMonth(
                    DateTime.Now.Year,
                    DateTime.Now.Month));
            links.Add(
                Url.LinkToDay(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day));
        }

        return Ok(new HomeDto { Links = links.ToArray() });
    }
}
