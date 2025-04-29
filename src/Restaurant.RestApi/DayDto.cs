using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1819:Properties should not return arrays",
    Justification = "DTO.")]
public record DayDto
{
    public LinkDto[]? Links { get; set; }
    public string? Date { get; set; }
    public TimeDto[]? Entries { get; set; }
}
