﻿using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;

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
        await using var service = new RestaurantApiFactory();
        var client = service.CreateClient();

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

    [Fact]
    public async Task HomeReturnsCorrectLinks()
    {
        await using var service = new RestaurantApiFactory();
        var client = service.CreateClient();

        var response =
            await client.GetAsync(new Uri("", UriKind.Relative));

        var rels = new[]
        {
            "urn:reservations",
            "urn:year"
        };
        var expectedRels = new HashSet<string?>(rels);
        var actual = await ParseHomeContent(response);
        var actualRels = actual!.Links!.Select(l => l.Rel).ToHashSet();
        Assert.Superset(expectedRels, actualRels);
        Assert.All(actual.Links!, AssertHrefAbsoluteUrl);
    }

    private static async Task<HomeDto?> ParseHomeContent(
        HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return CustomJsonSerializer.Deserialize<HomeDto>(json);
    }

    private static void AssertHrefAbsoluteUrl(LinkDto dto)
    {
        Assert.True(
            Uri.TryCreate(dto.Href, UriKind.Absolute, out var _),
            $"Not an absolute URL: {dto.Href}.");
    }
}
