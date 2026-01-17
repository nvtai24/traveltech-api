using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Extensions;

namespace TravelTechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            // Using extension method for standardized response
            return this.Success("Pong", "Service is running");
        }

        /// <summary>
        /// Example endpoint that throws an exception (for testing error handling)
        /// </summary>
        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new Exception("This is a test exception");
        }
    }
}
