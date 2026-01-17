using Microsoft.AspNetCore.Identity;
using TravelTechApi.Data;
using TravelTechApi.Entities;

namespace TravelTechApi.Extensions
{
    /// <summary>
    /// Extension methods for Identity configuration
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Add ASP.NET Core Identity with custom configuration
        /// </summary>
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign in settings
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
