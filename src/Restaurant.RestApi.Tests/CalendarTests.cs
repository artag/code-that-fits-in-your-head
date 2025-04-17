namespace Restaurant.RestApi.Tests;

public class CalendarTests
{
    [Fact]
    public async Task GetCurrentYear()
    {
        await using var service = new RestaurantApiFactory();

        var response = await service.GetCurrentYear();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
    }
}
