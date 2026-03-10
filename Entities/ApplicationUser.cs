using Microsoft.AspNetCore.Identity;

namespace TravelTechApi.Entities
{
    /// <summary>
    /// Application user model extending IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User's first name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string? LastName { get; set; }

        public Gender? Gender { get; set; }

        public DateTime? Dob { get; set; }

        public bool IsFirstLogin { get; set; } = true;

        /// <summary>
        /// Date when user was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when user was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property for refresh tokens
        /// </summary>
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public virtual ICollection<DestinationSharing> DestinationSharings { get; set; } = new List<DestinationSharing>();

        public string? AvatarId { get; set; }

        public virtual CloudinaryFileInfo? Avatar { get; set; }

        /// <summary>
        /// Navigation property for plan subscriptions
        /// </summary>
        public virtual ICollection<UserPlanSubscription> Subscriptions { get; set; } = new List<UserPlanSubscription>();

        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}
