using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1819:Properties should not return arrays",
    Justification = "DTO.")]
public record CalendarDto
{
    public LinkDto[]? Links { get; set; }

    public int Year { get; init; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public DayDto[]? Days { get; set; }
}
