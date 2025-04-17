using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    [HttpGet("{year}")]
    public ActionResult Get(int year)
    {
        return new OkObjectResult(
            new CalendarDto { Year = year });
    }
}
