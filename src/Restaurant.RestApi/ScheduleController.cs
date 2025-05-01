using Microsoft.AspNetCore.Mvc;

namespace Restaurant.RestApi;

[Route("schedule")]
public class ScheduleController : ControllerBase
{
    [HttpGet("{year}/{month}/{day}")]
    public ActionResult Get(int _, int __, int ___)
    {
        return new UnauthorizedResult();
    }
}
