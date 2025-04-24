using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "This class is instantiated via Reflection.")]
internal sealed class LinksFilter : IAsyncActionFilter
{
    public IUrlHelperFactory UrlHelperFactory { get; }

    public LinksFilter(IUrlHelperFactory urlHelperFactory)
    {
        UrlHelperFactory = urlHelperFactory;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var ctxAfter = await next().ConfigureAwait(false);
        if (!(ctxAfter.Result is OkObjectResult ok))
            return;

        var url = UrlHelperFactory.GetUrlHelper(ctxAfter);
        switch (ok.Value)
        {
            case CalendarDto calendarDto:
                AddLinks(calendarDto, url);
                break;
        }
    }

    private static void AddLinks(CalendarDto dto, IUrlHelper url)
    {
        if (dto.Month is null)
        {
            var date = new DateTime(dto.Year, 1, 1);
            var previousYear = date.AddYears(-1);
            var nextYear = date.AddYears(1);
            dto.Links = new[]
            {
                url.LinkToYear(previousYear.Year, "previous"),
                url.LinkToYear(nextYear.Year, "next")
            };
        }
        else if (dto.Day is null)
        {
            var date = new DateTime(dto.Year, dto.Month.Value, 1);
            var previousMonth = date.AddMonths(-1);
            var nextMonth = date.AddMonths(1);
            dto.Links = new[]
            {
                url.LinkToMonth(previousMonth.Year, previousMonth.Month, "previous"),
                url.LinkToMonth(nextMonth.Year, nextMonth.Month, "next")
                };
        }
        else
        {
            var date = new DateTime(dto.Year, dto.Month.Value, dto.Day.Value);
            var previousDay = date.AddDays(-1);
            var nextDay = date.AddDays(1);
            dto.Links = new[]
            {
                url.LinkToDay(previousDay.Year, previousDay.Month, previousDay.Day, "previous"),
                url.LinkToDay(nextDay.Year, nextDay.Month, nextDay.Day, "next")
            };
        }
    }
}
