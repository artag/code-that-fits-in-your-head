using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1819:Properties should not return arrays",
    Justification = "DTO.")]
public record HomeDto
{
    public LinkDto[]? Links { get; set; }
    public RestaurantDto[]? Restaurants { get; set; }
}
