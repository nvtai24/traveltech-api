using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using TravelTechApi.Common.Exceptions;

namespace TravelTechApi.Common.Exceptions
{
    /// <summary>
    /// Global exception handler middleware to catch and format all exceptions
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = httpContext.Response;
            response.ContentType = "application/json";

            ApiResponse<object> apiResponse;
            int statusCode;

            switch (exception)
            {
                case BaseException customException:
                    // Handle custom exceptions
                    statusCode = customException.StatusCode;
                    apiResponse = ApiResponse<object>.FailureResponse(
                        customException.Message,
                        customException.Errors
                    );
                    break;

                case UnauthorizedAccessException:
                    // Handle unauthorized access
                    statusCode = StatusCodes.Status401Unauthorized;
                    apiResponse = ApiResponse<object>.FailureResponse(
                        "Unauthorized access",
                        new List<ErrorDetail>
                        {
                            new ErrorDetail { Message = exception.Message }
                        }
                    );
                    break;

                case ArgumentException:
                    // Handle argument exceptions as bad requests
                    statusCode = StatusCodes.Status400BadRequest;
                    apiResponse = ApiResponse<object>.FailureResponse(
                        "Invalid argument",
                        new List<ErrorDetail>
                        {
                            new ErrorDetail { Message = exception.Message }
                        }
                    );
                    break;

                default:
                    // Handle all other exceptions as internal server errors
                    statusCode = StatusCodes.Status500InternalServerError;

                    // Don't expose internal error details in production
                    var isDevelopment = httpContext.RequestServices
                        .GetRequiredService<IWebHostEnvironment>()
                        .IsDevelopment();

                    var errorMessage = isDevelopment
                        ? exception.Message
                        : "An internal server error occurred";

                    var errors = isDevelopment && exception.StackTrace != null
                        ? new List<ErrorDetail>
                        {
                            new ErrorDetail
                            {
                                Message = exception.Message,
                                Code = exception.GetType().Name
                            }
                        }
                        : null;

                    apiResponse = ApiResponse<object>.FailureResponse(errorMessage, errors);
                    break;
            }

            response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await response.WriteAsync(
                JsonSerializer.Serialize(apiResponse, options),
                cancellationToken
            );

            return true; // Exception handled
        }
    }
}
