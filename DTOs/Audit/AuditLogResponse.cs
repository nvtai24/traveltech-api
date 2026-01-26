namespace TravelTechApi.DTOs.Audit
{
    /// <summary>
    /// Response DTO for audit log entry
    /// </summary>
    public class AuditLogResponse
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; } // Email or Name
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
