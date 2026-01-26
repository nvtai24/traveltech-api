using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.Audit;
using TravelTechApi.Services.Audit;

namespace TravelTechApi.Controllers
{
    [Route("api/audit-logs")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Get audit logs with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? userId = null,
            [FromQuery] string? action = null,
            [FromQuery] string? entityName = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var result = await _auditLogService.GetAuditLogsAsync(page, pageSize, userId, action, entityName, fromDate, toDate);
            return this.Success(result, "Audit logs retrieved successfully");
        }
    }
}
