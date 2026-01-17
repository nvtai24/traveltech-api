namespace TravelTechApi.Common
{
    /// <summary>
    /// Generic API response wrapper for consistent response structure
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Human-readable message about the operation
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The actual data payload
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// List of errors if any occurred
        /// </summary>
        public List<ErrorDetail>? Errors { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        /// <summary>
        /// Creates a failure response
        /// </summary>
        public static ApiResponse<T> FailureResponse(string message, List<ErrorDetail>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }

        /// <summary>
        /// Creates a failure response with a single error
        /// </summary>
        public static ApiResponse<T> FailureResponse(string message, string errorMessage)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail { Message = errorMessage }
                }
            };
        }
    }

    /// <summary>
    /// Non-generic API response for operations that don't return data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public new static ApiResponse CreateSuccess(string message = "Operation completed successfully")
        {
            var response = new ApiResponse();
            response.Success = true;
            response.Message = message;
            response.Data = null;
            response.Errors = null;
            return response;
        }

        /// <summary>
        /// Creates a failure response without data
        /// </summary>
        public new static ApiResponse CreateFailure(string message, List<ErrorDetail>? errors = null)
        {
            var response = new ApiResponse();
            response.Success = false;
            response.Message = message;
            response.Data = null;
            response.Errors = errors;
            return response;
        }
    }

    /// <summary>
    /// Detailed error information
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Field name that caused the error (for validation errors)
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error code for programmatic handling
        /// </summary>
        public string? Code { get; set; }
    }
}
