using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.User;

namespace TravelTechApi.Services.User
{
    /// <summary>
    /// Service interface for user management operations (admin)
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Get paginated list of users with optional search and role filter
        /// </summary>
        Task<PagedResult<AdminUserListItemResponse>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null, string? role = null);

        /// <summary>
        /// Get detailed user information by id
        /// </summary>
        Task<AdminUserResponse> GetUserByIdAsync(string userId);


        /// <summary>
        /// Lock user account
        /// </summary>
        Task LockUserAsync(string userId);

        /// <summary>
        /// Unlock user account
        /// </summary>
        Task UnlockUserAsync(string userId);

        /// <summary>
        /// Change user role
        /// </summary>
        Task<AdminUserResponse> ChangeUserRoleAsync(string userId, string newRole);

        /// <summary>
        /// Get all available roles
        /// </summary>
        Task<List<string>> GetAllRolesAsync();

        /// <summary>
        /// Delete user account
        /// </summary>
        Task DeleteUserAsync(string userId);
    }
}
