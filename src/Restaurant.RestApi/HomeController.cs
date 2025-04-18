﻿using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

/// <summary>
/// Home controller.
/// </summary>
[Route("")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Get method.
    /// </summary>
    public IActionResult Get()
    {
        return Ok(new HomeDto
        {
            Links = new[]
            {
                CreateReservationsLink(),
                CreateYearLink()
            }
        });
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
}
