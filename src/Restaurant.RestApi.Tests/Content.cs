using System.Text.Json;

namespace Restaurant.RestApi.Tests;

internal static class Content
{
    public static async Task<T?> ParseJsonContent<T>(
        this HttpResponseMessage msg)
    {
        var json = await msg.Content.ReadAsStringAsync();
        return CustomJsonSerializer.Deserialize<T>(json);
    }
}
