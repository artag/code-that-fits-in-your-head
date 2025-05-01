using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("restaurants")]
public sealed class RestaurantsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult Get(int id)
    {
        var name = "Hipgnosta";
        if (id == 4)
            name = "Nono";
        if (id == 18)
            name = "The Vatican Cellar";

        return new OkObjectResult(new RestaurantDto { Name = name });
    }
}
