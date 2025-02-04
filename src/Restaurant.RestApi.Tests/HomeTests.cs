using System.Diagnostics.CodeAnalysis;
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
    public async Task HomeIsOk()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client
            .GetAsync("");

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }
}
