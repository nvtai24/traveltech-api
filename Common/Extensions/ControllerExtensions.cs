using Microsoft.AspNetCore.Mvc;

namespace TravelTechApi.Common.Extensions
{
    /// <summary>
    /// Extension methods for controllers to easily return standardized API responses
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Returns a successful response with data
        /// </summary>
        public static IActionResult Success<T>(this ControllerBase controller, T data, string message = "Operation completed successfully")
        {
            return controller.Ok(ApiResponse<T>.SuccessResponse(data, message));
        }

        /// <summary>
        /// Returns a successful response without data
        /// </summary>
        public static IActionResult Success(this ControllerBase controller, string message = "Operation completed successfully")
        {
            return controller.Ok(ApiResponse.CreateSuccess(message));
        }

        /// <summary>
        /// Returns a created response with data
        /// </summary>
        public static IActionResult Created<T>(this ControllerBase controller, T data, string message = "Resource created successfully")
        {
            return controller.StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<T>.SuccessResponse(data, message)
            );
        }

        /// <summary>
        /// Returns a bad request response
        /// </summary>
        public static IActionResult BadRequest(this ControllerBase controller, string message, List<ErrorDetail>? errors = null)
        {
            return controller.BadRequest(ApiResponse<object>.FailureResponse(message, errors));
        }

        /// <summary>
        /// Returns a not found response
        /// </summary>
        public static IActionResult NotFound(this ControllerBase controller, string message)
        {
            return controller.NotFound(ApiResponse<object>.FailureResponse(message));
        }

        /// <summary>
        /// Returns an unauthorized response
        /// </summary>
        public static IActionResult Unauthorized(this ControllerBase controller, string message = "Unauthorized access")
        {
            return controller.Unauthorized(ApiResponse<object>.FailureResponse(message));
        }

        /// <summary>
        /// Returns a forbidden response
        /// </summary>
        public static IActionResult Forbidden(this ControllerBase controller, string message = "Access forbidden")
        {
            return controller.StatusCode(
                StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(message)
            );
        }

        /// <summary>
        /// Returns an internal server error response
        /// </summary>
        public static IActionResult InternalServerError(this ControllerBase controller, string message = "An internal server error occurred")
        {
            return controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse(message)
            );
        }

        public static IActionResult Failed(this ControllerBase controller, string message)
        {
            return controller.BadRequest(ApiResponse<object>.FailureResponse(message));
        }
    }
}
