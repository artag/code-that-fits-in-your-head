using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

public record HomeDto
{
    [SuppressMessage(
        "Performance",
        "CA1819:Properties should not return arrays",
        Justification = "DTO.")]
    public LinkDto[]? Links { get; set; }
}
