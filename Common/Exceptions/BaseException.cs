namespace TravelTechApi.Common.Exceptions
{
    /// <summary>
    /// Base exception for all custom exceptions in the application
    /// </summary>
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; }
        public List<ErrorDetail>? Errors { get; }

        protected BaseException(string message, int statusCode, List<ErrorDetail>? errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }

    /// <summary>
    /// Exception for bad request (400)
    /// </summary>
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message, List<ErrorDetail>? errors = null)
            : base(message, StatusCodes.Status400BadRequest, errors)
        {
        }
    }

    /// <summary>
    /// Exception for not found (404)
    /// </summary>
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }

    /// <summary>
    /// Exception for unauthorized access (401)
    /// </summary>
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized access")
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }

    /// <summary>
    /// Exception for forbidden access (403)
    /// </summary>
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "Access forbidden")
            : base(message, StatusCodes.Status403Forbidden)
        {
        }
    }

    /// <summary>
    /// Exception for conflict (409)
    /// </summary>
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict)
        {
        }
    }

    /// <summary>
    /// Exception for validation errors (422)
    /// </summary>
    public class ValidationException : BaseException
    {
        public ValidationException(string message, List<ErrorDetail> errors)
            : base(message, StatusCodes.Status422UnprocessableEntity, errors)
        {
        }

        public ValidationException(List<ErrorDetail> errors)
            : base("Validation failed", StatusCodes.Status422UnprocessableEntity, errors)
        {
        }
    }
}
