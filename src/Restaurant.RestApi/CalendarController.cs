using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("calendar")]
public class CalendarController : ControllerBase
{
    [HttpGet]
    public void Get()
    {
    }
}
