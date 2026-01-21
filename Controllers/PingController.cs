using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TravelTechApi.Common.Extensions;

namespace TravelTechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {

        private readonly ILogger<PingController> _logger;

        private readonly IConnectionMultiplexer _redis;

        public PingController(ILogger<PingController> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
        }

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

        [HttpGet("redis")]
        public IActionResult GetRedis()
        {
            var db = _redis.GetDatabase();
            db.StringSet("test", "123456");
            var value = db.StringGet("test");
            return this.Success("Redis is working", value.ToString());
        }
    }
}
