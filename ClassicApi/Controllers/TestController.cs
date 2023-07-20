using Microsoft.AspNetCore.Mvc;

namespace AK.HostingSpa.ClassicApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
	[HttpGet("[action]")]
	public IActionResult Get() => new NoContentResult();
}