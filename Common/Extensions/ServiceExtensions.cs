using TravelTechApi.Common.Settings;
using TravelTechApi.Services;

namespace TravelTechApi.Extensions
{
    /// <summary>
    /// Extension methods for application services registration
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Register application services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(Program).Assembly);

            // Register scoped services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<ICloudinaryService, Services.Cloudinary.CloudinaryService>();
            services.AddScoped<IRegionService, Services.Travel.RegionService>();
            services.AddScoped<ILocationService, Services.Travel.LocationService>();
            services.AddScoped<IDestinationService, Services.Travel.DestinationService>();

            return services;
        }

        /// <summary>
        /// Configure email settings
        /// </summary>
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            // Email Settings
            services.Configure<TravelTechApi.Common.Settings.EmailSettings>(
                configuration.GetSection("EmailSettings"));

            // JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Cloudinary Settings
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            return services;
        }
    }
}
