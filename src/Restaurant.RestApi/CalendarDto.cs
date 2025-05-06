using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1819:Properties should not return arrays",
    Justification = "DTO.")]
public record CalendarDto
{
    public LinkDto[]? Links { get; set; }

    public string? Name { get; set; }

    public int Year { get; init; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public DayDto[]? Days { get; set; }

    internal IPeriod ToPeriod()
    {
        if (Month is null)
            return Period.Year(Year);
        else if (Day is null)
            return Period.Month(Year, Month.Value);
        else
            return Period.Day(Year, Month.Value, Day.Value);
    }
}
