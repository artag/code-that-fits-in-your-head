using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi.Tests;

public class CalendarTests
{
    [Fact]
    public async Task GetCurrentYear()
    {
        var currentYear = DateTime.Now.Year;
        await using var service = new RestaurantApiFactory();

        var response = await service.GetCurrentYear();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
        var actual = await ParseCalendarContent(response);
        AssertCurrentYear(currentYear, actual!.Year);
    }

    private static async Task<CalendarDto?> ParseCalendarContent(
        HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return CustomJsonSerializer.Deserialize<CalendarDto>(json);
    }

    private static void AssertCurrentYear(int expected, int actual)
    {
        /* If a test runs just at midnight on December 31, the year could
         * increment during execution. Thus, while the current year is the
         * most reasonable expectation, the next year should also pass the
         * test. */
        Assert.InRange(actual, expected, expected + 1);
    }

    [Theory]
    [InlineData(2000, 366, 10)]
    [InlineData(2019, 365, 20)]
    [InlineData(2020, 366, 5)]
    [InlineData(2040, 366, 10)]
    [InlineData(2100, 365, 8)]
    public void GetYear(int year, int expectedDays, int tableSize)
    {
        var sut = new CalendarController(Table.Communal(tableSize));

        var actual = sut.Get(year);

        var ok = Assert.IsAssignableFrom<OkObjectResult>(actual);
        var dto = Assert.IsAssignableFrom<CalendarDto>(ok.Value);
        Assert.Equal(year, dto.Year);
        Assert.NotNull(dto.Days);
        var days = dto.Days;
        Assert.Equal(expectedDays, days?.Length);
        Assert.Equal(
            expectedDays,
            days!.Select(d => d.Date).Distinct().Count());
        Assert.All(
            dto.Days.Select(d => d.MaximumPartySize),
            i => Assert.Equal(tableSize, i));
    }
}
