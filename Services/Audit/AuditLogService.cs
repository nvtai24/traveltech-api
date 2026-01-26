using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Audit;
using TravelTechApi.DTOs.Common;
using TravelTechApi.Entities;

namespace TravelTechApi.Services.Audit
{
    /// <summary>
    /// Service implementation for audit logging
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<AuditLogService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task LogAsync(string? userId, string action, string entityName, string? entityId, string? details = null, string? ipAddress = null)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    EntityName = entityName,
                    EntityId = entityId,
                    Details = details,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.AuditLogs.AddAsync(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Verify that we don't block the main flow if logging fails
                _logger.LogError(ex, "Failed to create audit log for action {Action} on entity {EntityName}", action, entityName);
            }
        }

        public async Task<PagedResult<AuditLogResponse>> GetAuditLogsAsync(int page, int pageSize, string? userId = null, string? action = null, string? entityName = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Include(l => l.User)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(l => l.UserId == userId);
            }

            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(l => l.Action == action);
            }

            if (!string.IsNullOrEmpty(entityName))
            {
                query = query.Where(l => l.EntityName == entityName);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt <= toDate.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<AuditLogResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return PagedResult<AuditLogResponse>.Create(logs, totalCount, page, pageSize);
        }
    }
}
