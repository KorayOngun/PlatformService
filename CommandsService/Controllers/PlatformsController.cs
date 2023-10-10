using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;


[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    public PlatformsController()
    {

    }


    [HttpPost]
    public ActionResult TestInBoundConnection()
    {
        Console.WriteLine("--> Inbunt POST # Command Service");
        return Ok("Inbound test of from platforms controler");
    }
}