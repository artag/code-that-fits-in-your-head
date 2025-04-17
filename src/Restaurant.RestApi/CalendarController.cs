using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return new OkObjectResult(
            new CalendarDto { Year = DateTime.Now.Year });
    }
}
