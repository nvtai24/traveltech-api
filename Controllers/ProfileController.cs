using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelTechApi.Common.Extensions;
using TravelTechApi.Data;
using TravelTechApi.DTOs.Auth;
using TravelTechApi.DTOs.Profile;
using TravelTechApi.Entities;
using TravelTechApi.Services.File;
using System.Security.Claims;
using AutoMapper;
using TravelTechApi.Services.UserPlanSubscription;

namespace TravelTechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserPlanSubscriptionService _userPlanSubscriptionService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IFileService fileService,
            ApplicationDbContext context,
            ILogger<ProfileController> logger,
            IMapper mapper,
            IUserPlanSubscriptionService userPlanSubscriptionService)
        {
            _userManager = userManager;
            _fileService = fileService;
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _userPlanSubscriptionService = userPlanSubscriptionService;
        }

        /// <summary>
        /// Get current user information (requires authentication)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var firstName = User.FindFirstValue(ClaimTypes.GivenName);
            var lastName = User.FindFirstValue(ClaimTypes.Surname);
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var user = _context.Users
                .AsNoTracking() // Read-only
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound("User not found");

            var userDto = _mapper.Map<UserResponse>(user);

            var currentPlan = await _userPlanSubscriptionService.GetCurrentPlanAsync(userId!);
            userDto.SubscriptionPlan = currentPlan.Name;

            // Roles are not loaded in this simple query usually, unless Included.
            // But we can get roles from claims or UserManager.
            // Claims are already there.
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Any())
            {
                // Fallback if claims missing (unlikely if authorized)
                // roles = await _userManager.GetRolesAsync(user); // Sync here or async?
                // Let's stick to claims for roles to avoid extra DB calls if possible, or just accept empty if claims missing.
            }
            userDto.Roles = roles;
            userDto.PhoneNumber = phoneNumber ?? string.Empty; // Claims might have it, or DB user has it? DB user has it.
                                                               // Map overrides properties.
                                                               // UserResponse mapping already handles basics.

            _logger.LogDebug("Retrieved current user info for: {UserId}", userId);

            return this.Success(userDto, "User retrieved successfully");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return this.Unauthorized("User not found");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return this.NotFound("User not found");
            }




            bool isUpdated = false;

            if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.FirstName)
            {
                user.FirstName = request.FirstName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.LastName)
            {
                user.LastName = request.LastName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = request.PhoneNumber;
                isUpdated = true;
            }

            if (request.Gender.HasValue && request.Gender != user.Gender)
            {
                user.Gender = request.Gender;
                isUpdated = true;
            }

            if (request.Dob.HasValue && request.Dob != user.Dob)
            {
                user.Dob = request.Dob;
                isUpdated = true;
            }

            if (request.Avatar != null)
            {
                // Delete old avatar if exists
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var fileKey = ExtractFileKeyFromUrl(user.AvatarUrl);
                    if (!string.IsNullOrEmpty(fileKey))
                    {
                        await _fileService.DeleteImageAsync(fileKey);
                    }
                }

                // Upload new avatar
                var uploadUrl = await _fileService.UploadImageAsync(request.Avatar, "avatars");
                user.AvatarUrl = uploadUrl;
                isUpdated = true;
            }

            if (isUpdated)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("User profile updated for {UserId}", userId);
            }

            var response = _mapper.Map<UserResponse>(user);

            var currentPlan = await _userPlanSubscriptionService.GetCurrentPlanAsync(userId!);
            response.SubscriptionPlan = currentPlan.Name;
            // Handle role mapping if not covered by mapper for this instance (standard UserResponse usually has it)
            // But Map<UserResponse>(user) might miss Roles if they are not loaded or if logic is complex. 
            // UserResponse.Roles is List<string>. 
            // We can fetch roles if needed or keep it empty for profile update response if acceptable.
            // Let's ensure roles are preserved if the client needs them, but usually Profile update just returns user info.
            var roles = await _userManager.GetRolesAsync(user);
            response.Roles = roles.ToList();



            return this.Success(response, "Profile updated successfully");
        }

        private string ExtractFileKeyFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            try
            {
                var uri = new Uri(url);
                return uri.AbsolutePath.TrimStart('/');
            }
            catch { }
            return string.Empty;
        }
    }
}
