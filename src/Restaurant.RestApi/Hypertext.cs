using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

internal static class Hypertext
{
    private static readonly UrlBuilder Reservations =
        new UrlBuilder()
            .WithAction(nameof(ReservationsController.Post))
            .WithController(nameof(ReservationsController));
    private static readonly UrlBuilder Calendar =
        new UrlBuilder()
            .WithAction(nameof(CalendarController.Get))
            .WithController(nameof(CalendarController));

    internal static LinkDto Link(this Uri uri, string rel)
    {
        return new LinkDto { Rel = rel, Href = uri.ToString() };
    }

    internal static LinkDto LinkToReservations(this IUrlHelper url)
    {
        return Reservations.BuildAbsolute(url).Link("urn:reservations");
    }

    internal static LinkDto LinkToYear(this IUrlHelper url, int year)
    {
        return Calendar
            .WithValues(new { year })
            .BuildAbsolute(url)
            .Link("urn:year");
    }

    internal static LinkDto LinkToMonth(
        this IUrlHelper url,
        int year,
        int month)
    {
        return Calendar
            .WithValues(new { year, month })
            .BuildAbsolute(url)
            .Link("urn:month");
    }

    internal static LinkDto LinkToDay(
        this IUrlHelper url,
        int year,
        int month,
        int day)
    {
        return Calendar
            .WithValues(new { year, month, day })
            .BuildAbsolute(url)
            .Link("urn:day");
    }
}
