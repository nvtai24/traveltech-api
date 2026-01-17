using System.ComponentModel.DataAnnotations;

namespace TravelTechApi.Entities
{
    /// <summary>
    /// Refresh token model for JWT token refresh
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User ID that owns this refresh token
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The refresh token string
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// JWT ID that this refresh token is associated with
        /// </summary>
        public string JwtId { get; set; } = string.Empty;

        /// <summary>
        /// Whether this token has been used
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Whether this token has been revoked
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// When this token was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this token expires
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Navigation property to user
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
    }
}
