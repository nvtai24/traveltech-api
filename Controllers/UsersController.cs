using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Extensions;
using TravelTechApi.DTOs.User;
using TravelTechApi.Services.User;

namespace TravelTechApi.Controllers
{
    /// <summary>
    /// Controller for user management operations (Admin only)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserManagementService userManagementService, ILogger<UsersController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of users
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term (email, first name, last name)</param>
        /// <param name="role">Optional role filter</param>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? role = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _userManagementService.GetAllUsersAsync(page, pageSize, searchTerm, role);
            return this.Success(result, "Users retrieved successfully");
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userManagementService.GetUserByIdAsync(id);
            return this.Success(result, "User retrieved successfully");
        }

        /// <summary>
        /// Update user information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userManagementService.UpdateUserAsync(id, request);
            _logger.LogInformation("Admin updated user {UserId}", id);
            return this.Success(result, "User updated successfully");
        }

        /// <summary>
        /// Lock user account
        /// </summary>
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser(string id)
        {
            await _userManagementService.LockUserAsync(id);
            _logger.LogInformation("Admin locked user {UserId}", id);
            return this.Success("User locked successfully");
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            await _userManagementService.UnlockUserAsync(id);
            _logger.LogInformation("Admin unlocked user {UserId}", id);
            return this.Success("User unlocked successfully");
        }

        /// <summary>
        /// Change user role
        /// </summary>
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] ChangeUserRoleRequest request)
        {
            var result = await _userManagementService.ChangeUserRoleAsync(id, request.Role);
            _logger.LogInformation("Admin changed user {UserId} role to {Role}", id, request.Role);
            return this.Success(result, "User role changed successfully");
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _userManagementService.GetAllRolesAsync();
            return this.Success(result, "Roles retrieved successfully");
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userManagementService.DeleteUserAsync(id);
            _logger.LogInformation("Admin deleted user {UserId}", id);
            return this.Success("User deleted successfully");
        }
    }
}
