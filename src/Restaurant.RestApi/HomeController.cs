using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

/// <summary>
/// Home controller.
/// </summary>
[Route("")]
public class HomeController : ControllerBase
{
    /// <summary>
    /// Get method.
    /// </summary>
    public IActionResult Get()
    {
        return new OkObjectResult(new HomeDto());
    }
}
