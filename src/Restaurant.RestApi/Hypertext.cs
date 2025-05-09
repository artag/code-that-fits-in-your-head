﻿using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

public static class Hypertext
{
    private static readonly UrlBuilder Reservations =
        new UrlBuilder()
            .WithAction(nameof(ReservationsController.Post))
            .WithController(nameof(ReservationsController));
    private readonly static UrlBuilder Restaurants =
        new UrlBuilder()
            .WithAction(nameof(RestaurantsController.Get))
            .WithController(nameof(RestaurantsController));
    private static readonly UrlBuilder Calendar =
        new UrlBuilder()
            .WithController(nameof(CalendarController));
    private readonly static UrlBuilder Schedule =
        new UrlBuilder()
            .WithAction(nameof(ScheduleController.Get))
            .WithController(nameof(ScheduleController));

    internal static LinkDto Link(this Uri uri, string rel)
    {
        return new LinkDto { Rel = rel, Href = uri.ToString() };
    }

    internal static LinkDto LinkToReservations(
        this IUrlHelper url,
        int restaurantId)
    {
        return Reservations
            .WithValues(new { restaurantId })
            .BuildAbsolute(url)
            .Link("urn:reservations");
    }

    internal static LinkDto LinkToRestaurant(this IUrlHelper url, int id)
    {
        return Restaurants
            .WithValues(new { id })
            .BuildAbsolute(url)
            .Link("urn:restaurant");
    }

    internal static LinkDto LinkToYear(
        this IUrlHelper url,
        int restaurantId,
        int year)
    {
        return url.LinkToYear(restaurantId, year, "urn:year");
    }

    internal static LinkDto LinkToYear(
        this IUrlHelper url,
        int restaurantId,
        int year,
        string rel)
    {
        return Calendar
            .WithAction(nameof(CalendarController.GetYear))
            .WithValues(new { restaurantId, year })
            .BuildAbsolute(url)
            .Link(rel);
    }

    internal static LinkDto LinkToMonth(
        this IUrlHelper url,
        int restaurantId,
        int year,
        int month)
    {
        return url.LinkToMonth(restaurantId, year, month, "urn:month");
    }

    internal static LinkDto LinkToMonth(
        this IUrlHelper url,
        int restaurantId,
        int year,
        int month,
        string rel)
    {
        return Calendar
            .WithAction(nameof(CalendarController.GetMonth))
            .WithValues(new { restaurantId, year, month })
            .BuildAbsolute(url)
            .Link(rel);
    }

    internal static LinkDto LinkToDay(
        this IUrlHelper url,
        int restaurantId,
        int year,
        int month,
        int day)
    {
        return url.LinkToDay(restaurantId, year, month, day, "urn:day");
    }

    internal static LinkDto LinkToDay(
        this IUrlHelper url,
        int restaurantId,
        int year,
        int month,
        int day,
        string rel)
    {
        return Calendar
            .WithAction(nameof(CalendarController.GetDay))
            .WithValues(new { restaurantId, year, month, day })
            .BuildAbsolute(url)
            .Link(rel);
    }

    internal static LinkDto LinkToPeriod(
        this IUrlHelper url,
        IPeriod period,
        string rel)
    {
        var values = period.Accept(new ValuesVisitor());
        return Calendar
            .WithValues(values)
            .BuildAbsolute(url)
            .Link(rel);
    }

    private sealed class ValuesVisitor : IPeriodVisitor<object>
    {
        public object VisitYear(int year)
        {
            return new { year };
        }

        public object VisitMonth(int year, int month)
        {
            return new { year, month };
        }

        public object VisitDay(int year, int month, int day)
        {
            return new { year, month, day };
        }
    }

    internal static LinkDto LinkToSchedule(
        this IUrlHelper url,
        int year,
        int month,
        int day)
    {
        return Schedule
            .WithValues(new { year, month, day })
            .BuildAbsolute(url)
            .Link("urn:schedule");
    }

    public static Uri FindAddress(
        this IEnumerable<LinkDto>? links,
        string rel)
    {
        var address = links?.Single(l => l.Rel == rel).Href;
        if (address is null)
            throw new InvalidOperationException(
                $"Address for relationship type \"{rel}\" not found.");

        return new Uri(address);
    }
}
