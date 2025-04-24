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
            dto.Links = new[]
            {
                new LinkDto
                {
                    Rel = "previous",
                    Href = url.LinkToYear(dto.Year - 1).Href
                },
                new LinkDto
                {
                    Rel = "next",
                    Href = url.LinkToYear(dto.Year + 1).Href
                }
            };
        }
        else
        {
            dto.Links = new[]
            {
                new LinkDto
                {
                    Rel = "previous",
                    Href =
                        url.LinkToMonth(dto.Year, dto.Month.Value - 1).Href
                },
                new LinkDto
                {
                    Rel = "next",
                    Href = url.LinkToMonth(dto.Year, dto.Month.Value + 1).Href
                }
            };
        }
    }
}
