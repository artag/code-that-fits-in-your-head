using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

public record DayDto
{
    [SuppressMessage(
        "Performance",
        "CA1819:Properties should not return arrays",
        Justification = "DTO.")]
    public LinkDto[]? Links { get; set; }
    public string? Date { get; set; }
    public int MaximumPartySize { get; set; }
}
