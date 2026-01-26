using TravelTechApi.DTOs.Audit;
using TravelTechApi.DTOs.Common;

namespace TravelTechApi.Services.Audit
{
    /// <summary>
    /// Service interface for audit logging
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Create a new audit log entry
        /// </summary>
        /// <param name="userId">User ID performing the action (nullable)</param>
        /// <param name="action">Action performed (e.g., Create, Update, Delete)</param>
        /// <param name="entityName">Name of the entity affected</param>
        /// <param name="entityId">ID of the entity affected</param>
        /// <param name="details">Additional details (e.g., JSON of changes)</param>
        /// <param name="ipAddress">IP address of the user</param>
        Task LogAsync(string? userId, string action, string entityName, string? entityId, string? details = null, string? ipAddress = null);

        /// <summary>
        /// Get paginated audit logs for admin
        /// </summary>
        Task<PagedResult<AuditLogResponse>> GetAuditLogsAsync(int page, int pageSize, string? userId = null, string? action = null, string? entityName = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
