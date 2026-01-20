using TravelTechApi.Common.Settings;
using TravelTechApi.Services.AI;
using TravelTechApi.Services.Auth;
using TravelTechApi.Services.Cloudinary;
using TravelTechApi.Services.Destination;
using TravelTechApi.Services.Email;
using TravelTechApi.Services.Plan;

namespace TravelTechApi.Common.Extensions
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

            // Register application services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IDestinationService, DestinationService>();

            // AI and Plan services
            services.AddScoped<IAIService, OpenAIService>();
            services.AddScoped<IPlanGenerationService, PlanGenerationService>();

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

            // AI Settings
            services.Configure<AISettings>(configuration.GetSection("AISettings"));

            return services;
        }
    }
}
