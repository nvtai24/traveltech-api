// using Microsoft.AspNetCore.Mvc;
// using TravelTechApi.Common;
// using TravelTechApi.Common.Extensions;
// using TravelTechApi.Common.Exceptions;

// namespace TravelTechApi.Controllers
// {
//     /// <summary>
//     /// Example controller demonstrating how to use the standardized API response
//     /// </summary>
//     [Route("api/[controller]")]
//     [ApiController]
//     public class ExampleController : ControllerBase
//     {
//         // Example 1: Return success with data
//         [HttpGet("user/{id}")]
//         public IActionResult GetUser(int id)
//         {
//             var user = new { Id = id, Name = "John Doe", Email = "john@example.com" };
//             return this.Success(user, "User retrieved successfully");
//         }

//         // Example 2: Return success without data
//         [HttpPost("send-email")]
//         public IActionResult SendEmail()
//         {
//             // Send email logic here...
//             return this.Success("Email sent successfully");
//         }

//         // Example 3: Return created response
//         [HttpPost("user")]
//         public IActionResult CreateUser()
//         {
//             var newUser = new { Id = 123, Name = "Jane Doe" };
//             return this.Created(newUser, "User created successfully");
//         }

//         // Example 4: Throw custom exception (will be caught by global handler)
//         [HttpGet("not-found/{id}")]
//         public IActionResult GetNonExistentItem(int id)
//         {
//             throw new NotFoundException($"Item with ID {id} not found");
//         }

//         // Example 5: Throw validation exception
//         [HttpPost("validate")]
//         public IActionResult ValidateData([FromBody] string data)
//         {
//             if (string.IsNullOrEmpty(data))
//             {
//                 var errors = new List<ErrorDetail>
//                 {
//                     new ErrorDetail
//                     {
//                         Field = "data",
//                         Message = "Data cannot be empty",
//                         Code = "REQUIRED_FIELD"
//                     }
//                 };
//                 throw new ValidationException("Validation failed", errors);
//             }

//             return this.Success("Data is valid");
//         }

//         // Example 6: Return bad request manually
//         [HttpGet("manual-error")]
//         public IActionResult ManualError()
//         {
//             var errors = new List<ErrorDetail>
//             {
//                 new ErrorDetail { Message = "Something went wrong" }
//             };
//             return this.BadRequest("Invalid request", errors);
//         }
//     }
// }
