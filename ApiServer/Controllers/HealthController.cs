using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.Now,
                message = "FU News Management API is running successfully!",
                endpoints = new
                {
                    rest = "Available at /api/* endpoints",
                    odata = "Available at /odata/* endpoints",
                    swagger = "Available at /swagger",
                    metadata = "Available at /odata/$metadata"
                }
            });
        }
    }
}

