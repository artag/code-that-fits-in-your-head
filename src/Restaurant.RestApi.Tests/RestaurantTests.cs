namespace Restaurant.RestApi.Tests;

public class RestaurantTests
{
    [Theory]
    [InlineData("Hipgnosta")]
    [InlineData("Nono")]
    [InlineData("The Vatican Cellar")]
    public async Task GetRestaurant(string name)
    {
        await using var service = new SelfHostedService();

        var response = await service.GetRestaurant(name);

        Assert.True(
            response.IsSuccessStatusCode,
            $"Actual status code: {response.StatusCode}.");
        var content = await response.ParseJsonContent<RestaurantDto>();
        Assert.Equal(name, content!.Name);
    }
}
