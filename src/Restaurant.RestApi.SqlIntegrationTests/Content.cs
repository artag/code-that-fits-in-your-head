using System.Text.Json;

namespace Restaurant.RestApi.SqlIntegrationTests;

internal static class Content
{
    private static readonly JsonSerializerOptions _options =
        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<T?> ParseJsonContent<T>(
        this HttpResponseMessage msg)
    {
        var json = await msg.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
