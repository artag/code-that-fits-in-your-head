using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    private readonly IRestaurantDatabase _database;

    public RestaurantsController(IRestaurantDatabase database)
    {
        _database = database;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var name = await _database.GetName(id).ConfigureAwait(false);
        return new OkObjectResult(new RestaurantDto { Name = name });
    }
}
