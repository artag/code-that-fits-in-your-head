using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Restaurant.RestApi.Tests;

/// <summary>
/// Tests for home resource.
/// </summary>
public class HomeTests
{
    /// <summary>
    /// Home returns OK status code.
    /// </summary>
    [Fact]
    [SuppressMessage(
        "Usage",
        "CA2234:Pass system uri objects instead of strings",
        Justification = "URL isn't passed as variable, but as literal.")]
    public async Task HomeReturnsJson()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "");
        request.Headers.Accept.ParseAdd(MediaTypeNames.Application.Json);
        var response = await client.SendAsync(request);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
        Assert.Equal(
            MediaTypeNames.Application.Json,
            response.Content.Headers.ContentType?.MediaType);
    }
}
