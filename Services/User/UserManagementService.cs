using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common.Constants;
using TravelTechApi.Common.Exceptions;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Common;
using TravelTechApi.DTOs.User;
using TravelTechApi.Entities;
using TravelTechApi.Services.UserPlanSubscription;
using TravelTechApi.Services.Auth;

namespace TravelTechApi.Services.User
{
    /// <summary>
    /// Service implementation for user management operations (admin)
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserPlanSubscriptionService _userPlanSubscriptionService;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IMapper _mapper;
        private readonly Services.Audit.IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;

        public UserManagementService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserPlanSubscriptionService userPlanSubscriptionService,
            ILogger<UserManagementService> logger,
            IMapper mapper,
            Services.Audit.IAuditLogService auditLogService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _userPlanSubscriptionService = userPlanSubscriptionService;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<AdminUserListItemResponse>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null, string? role = null)
        {
            var query = _context.Users
                .Include(u => u.Avatar)
                .AsNoTracking()
                .AsQueryable();

            // Search by email, first name, or last name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                    ((u.FirstName ?? "") + " " + (u.LastName ?? "")).ToLower().Contains(term) ||
                    ((u.LastName ?? "") + " " + (u.FirstName ?? "")).ToLower().Contains(term));
            }

            // Filter by role before pagination
            if (!string.IsNullOrWhiteSpace(role))
            {
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity != null)
                {
                    var userIdsInRole = await _context.UserRoles
                        .Where(ur => ur.RoleId == roleEntity.Id)
                        .Select(ur => ur.UserId)
                        .ToListAsync();

                    query = query.Where(u => userIdsInRole.Contains(u.Id));
                }
                else
                {
                    // Role doesn't exist, return empty result
                    return PagedResult<AdminUserListItemResponse>.Create(new List<AdminUserListItemResponse>(), 0, page, pageSize);
                }
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userResponses = new List<AdminUserListItemResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var response = _mapper.Map<AdminUserListItemResponse>(user);
                response.Roles = roles.ToList();
                response.SubscriptionPlan = await _userPlanSubscriptionService.GetCurrentPlanNameAsync(user.Id);

                userResponses.Add(response);
            }

            return PagedResult<AdminUserListItemResponse>.Create(userResponses, totalCount, page, pageSize);
        }

        public async Task<AdminUserResponse> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' not found");
            }

            var response = _mapper.Map<AdminUserResponse>(user);
            response.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            response.SubscriptionPlan = await _userPlanSubscriptionService.GetCurrentPlanNameAsync(user.Id);

            return response;
        }

        public async Task LockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' not found");
            }

            // Prevent locking admin users
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(AppRoles.Admin))
            {
                throw new BadRequestException("Cannot lock an admin account");
            }

            // Lock the user for 100 years (effectively permanent)
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to lock user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Admin locked user {UserId}", userId);

            await _auditLogService.LogAsync(
                _currentUserService.UserId,
                "Lock",
                "User",
                userId,
                "Locked user account indefinitely"
            );
        }

        public async Task UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' not found");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to unlock user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Admin unlocked user {UserId}", userId);

            await _auditLogService.LogAsync(
                _currentUserService.UserId,
                "Unlock",
                "User",
                userId,
                "Unlocked user account"
            );
        }

        public async Task<AdminUserResponse> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' not found");
            }

            // Validate role
            var validRoles = AppRoles.GetAllRoles();
            var dbRoles = await _roleManager.Roles.ToListAsync();
            if (!validRoles.Contains(newRole) || !dbRoles.Any(r => r.Name == newRole))
            {
                throw new BadRequestException($"Invalid role '{newRole}'. Valid roles: {string.Join(", ", validRoles)}");
            }

            // Remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    throw new BadRequestException($"Failed to remove current roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
                }
            }

            // Add new role
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                throw new BadRequestException($"Failed to add role: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Admin changed user {UserId} role to {Role}", userId, newRole);

            await _auditLogService.LogAsync(
                _currentUserService.UserId,
                "ChangeRole",
                "User",
                userId,
                $"Changed role to {newRole}"
            );

            return await GetUserByIdAsync(userId);
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name)
                .Where(n => n != null)
                .ToListAsync() as List<string>;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException($"User with id '{userId}' not found");
            }

            // Prevent deleting admin users
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(AppRoles.Admin))
            {
                throw new BadRequestException("Cannot delete an admin account");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new BadRequestException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("Admin deleted user {UserId}", userId);

            await _auditLogService.LogAsync(
                _currentUserService.UserId,
                "Delete",
                "User",
                userId,
                "Deleted user account"
            );
        }
    }
}
