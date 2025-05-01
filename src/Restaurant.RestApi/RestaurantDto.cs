using System.Diagnostics.CodeAnalysis;

namespace Restaurant.RestApi;

[SuppressMessage(
    "Performance",
    "CA1819:Properties should not return arrays",
    Justification = "DTO.")]
public class RestaurantDto
{
    public LinkDto[]? Links { get; set; }
    public string? Name { get; set; }
}
