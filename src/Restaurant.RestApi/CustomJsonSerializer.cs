using System.Text.Json;

namespace Restaurant.RestApi;

public static class CustomJsonSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, SerializerOptions);
}
