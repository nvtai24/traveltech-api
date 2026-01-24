using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TravelTechApi.Common.Filters;

public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var user = context.HttpContext.User;

        // Get basic request info
        var method = request.Method;
        var path = request.Path;
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : "";

        // Get user info if authenticated
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        var userEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? "N/A";

        // Get action parameters
        var actionParameters = string.Join(", ",
            context.ActionArguments.Select(arg => $"{arg.Key}={arg.Value}"));

        // Build log message
        var logMessage = $"API Request - Method: {method}, Endpoint: {path}{queryString}, " +
                        $"User: {userId} ({userEmail})";

        if (!string.IsNullOrEmpty(actionParameters))
        {
            logMessage += $", Parameters: {actionParameters}";
        }

        _logger.LogInformation(logMessage);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            _logger.LogWarning($"API Response - Failed with Exception: {context.Exception.Message}");
        }
        else
        {
            // // Optional: Log response info if needed
            var statusCode = context.HttpContext.Response.StatusCode;
            _logger.LogInformation($"API Response - Status: {statusCode}");
        }
    }
}
