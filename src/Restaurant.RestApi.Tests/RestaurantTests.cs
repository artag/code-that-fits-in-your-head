﻿namespace Restaurant.RestApi.Tests;

public class RestaurantTests
{
    [Theory]
    [InlineData("Hipgnosta")]
    [InlineData("Nono")]
    [InlineData("The Vatican Cellar")]
    public async Task GetRestaurant(string name)
    {
        await using var service = new SelfHostedApi();

        var response = await service.GetRestaurant(name);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
        var content = await response.ParseJsonContent<RestaurantDto>();
        Assert.Equal(name, content!.Name);
    }

    [Theory]
    [InlineData("Hipgnosta")]
    [InlineData("Nono")]
    [InlineData("The Vatican Cellar")]
    public async Task RestaurantReturnsCorrectLinks(string name)
    {
        await using var service = new SelfHostedApi();

        var response = await service.GetRestaurant(name);

        var urls = new[]
        {
            "urn:reservations",
            "urn:year",
            "urn:month",
            "urn:day"
        };
        var expected = new HashSet<string?>(urls);
        var actual = await response.ParseJsonContent<RestaurantDto>();
        var actualRels = actual!.Links!.Select(l => l.Rel).ToHashSet();
        Assert.Superset(expected, actualRels);
        Assert.All(actual.Links!, AssertHrefAbsoluteUrl);
    }

    private static void AssertHrefAbsoluteUrl(LinkDto dto)
    {
        Assert.True(
            Uri.TryCreate(dto.Href, UriKind.Absolute, out var _),
            $"Not an absolute URL: {dto.Href}.");
    }

    [Fact]
    public async Task ReserveTableAtNono()
    {
        await using var service = new SelfHostedApi();
        var dto = Some.Reservation.ToDto();
        dto.Quantity = 6;

        var response = await service.PostReservation("Nono", dto);

        var date = Some.Reservation.At;
        await AssertRemainingCapacity(service, date, "Nono", 4);
        await AssertRemainingCapacity(service, date, "Hipgnosta", 10);
    }

    private static async Task AssertRemainingCapacity(
        SelfHostedApi service,
        DateTime date,
        string name,
        int expected)
    {
        var response =
            await service.GetDay(name, date.Year, date.Month, date.Day);
        var day = await response.ParseJsonContent<CalendarDto>();
        Assert.All(
            day!.Days!.Single().Entries!,
            e => Assert.Equal(expected, e.MaximumPartySize));
    }
}
