using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

public record CalendarDto
{
    public int Year { get; init; }

    [SuppressMessage(
        "Performance",
        "CA1819:Properties should not return arrays",
        Justification = "DTO.")]
    public DayDto[]? Days { get; set; }
}
