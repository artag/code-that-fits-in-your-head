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
        return Ok(new { message = "Hello, World!" });
    }
}
