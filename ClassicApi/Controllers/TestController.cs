using Microsoft.AspNetCore.Mvc;

namespace AK.HostingSpa.ClassicApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TestController : ControllerBase
{
	[HttpGet("")]
	public IActionResult Get() => new NoContentResult();
}