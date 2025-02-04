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
    public async Task HomeIsOk()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client
            .GetAsync(new Uri("", UriKind.Relative));

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }
}
