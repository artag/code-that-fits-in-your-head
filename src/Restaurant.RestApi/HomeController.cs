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
        const string controllerName = nameof(ReservationsController);
        var controller = controllerName.Remove(
            controllerName.LastIndexOf(
                "Controller",
                StringComparison.Ordinal));

        var href = Url.Action(
            nameof(ReservationsController.Post),
            controller,
            null,
            Url.ActionContext.HttpContext.Request.Scheme,
            Url.ActionContext.HttpContext.Request.Host.ToUriComponent());
        return new LinkDto
        {
            Rel = "urn:reservations",
            Href = href
        };
    }

    private LinkDto CreateYearLink()
    {
        const string controllerName = nameof(CalendarController);
        var controller = controllerName.Remove(
            controllerName.LastIndexOf(
                "Controller",
                StringComparison.Ordinal));

        var href = Url.Action(
            nameof(CalendarController.Get),
            controller,
            new { year = DateTime.Now.Year },
            Url.ActionContext.HttpContext.Request.Scheme,
            Url.ActionContext.HttpContext.Request.Host.ToUriComponent());

        return new LinkDto
        {
            Rel = "urn:year",
            Href = href
        };
    }

    private LinkDto CreateMonthLink()
    {
        const string controllerName = nameof(CalendarController);
        var controller = controllerName.Remove(
            controllerName.LastIndexOf(
                "Controller",
                StringComparison.Ordinal));

        var href = Url.Action(
            nameof(CalendarController.Get),
            controller,
            new { year = DateTime.Now.Year, month = DateTime.Now.Month },
            Url.ActionContext.HttpContext.Request.Scheme,
            Url.ActionContext.HttpContext.Request.Host.ToUriComponent());

        return new LinkDto
        {
            Rel = "urn:month",
            Href = href
        };
    }

    private LinkDto CreateDayLink()
    {
        const string controllerName = nameof(CalendarController);
        var controller = controllerName.Remove(
            controllerName.LastIndexOf(
                "Controller",
                StringComparison.Ordinal));

        var href = Url.Action(
            nameof(CalendarController.Get),
            controller,
            new { year = DateTime.Now.Year, month = DateTime.Now.Month },
            Url.ActionContext.HttpContext.Request.Scheme,
            Url.ActionContext.HttpContext.Request.Host.ToUriComponent());

        return new LinkDto
        {
            Rel = "urn:day",
            Href = href
        };
    }
}
