using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

/// <summary>
/// Home controller.
/// </summary>
[Route("")]
public class HomeController : ControllerBase
{
    private readonly IRestaurantDatabase _database;

    public HomeController(IRestaurantDatabase database)
    {
        _database = database;
    }

    /// <summary>
    /// Get method.
    /// </summary>
    public async Task<ActionResult> Get()
    {
        var names = await _database.GetAllNames().ConfigureAwait(false);
        var restaurants = names
            .Select(n => new RestaurantDto { Name = n })
            .ToArray();

        return new OkObjectResult(
            new HomeDto { Restaurants = restaurants });
    }
}
