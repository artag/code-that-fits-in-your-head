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
        var links = new List<LinkDto> { CreateReservationsLink() };
        if (_enableCalendar)
        {
            links.Add(CreateYearLink());
            links.Add(CreateMonthLink());
            links.Add(CreateDayLink());
        }

        return Ok(new HomeDto { Links = links.ToArray() });
    }

    private LinkDto CreateReservationsLink()
    {
        return new UrlBuilder()
            .WithAction(nameof(ReservationsController.Post))
            .WithController(nameof(ReservationsController))
            .BuildAbsolute(Url)
            .Link("urn:reservations");
    }

    private LinkDto CreateYearLink()
    {
        return new UrlBuilder()
            .WithAction(nameof(CalendarController.Get))
            .WithController(nameof(CalendarController))
            .WithValues(new { year = DateTime.Now.Year })
            .BuildAbsolute(Url)
            .Link("urn:year");
    }

    private LinkDto CreateMonthLink()
    {
        return new UrlBuilder()
            .WithAction(nameof(CalendarController.Get))
            .WithController(nameof(CalendarController))
            .WithValues(new
            {
                year = DateTime.Now.Year,
                month = DateTime.Now.Month
            })
            .BuildAbsolute(Url)
            .Link("urn:month");
    }

    private LinkDto CreateDayLink()
    {
        return new UrlBuilder()
            .WithAction(nameof(CalendarController.Get))
            .WithController(nameof(CalendarController))
            .WithValues(new
            {
                year = DateTime.Now.Year,
                month = DateTime.Now.Month,
                day = DateTime.Now.Day
            })
            .BuildAbsolute(Url)
            .Link("urn:day");
    }
}
