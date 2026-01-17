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

            return services;
        }

        /// <summary>
        /// Configure email settings
        /// </summary>
        public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TravelTechApi.Common.Settings.EmailSettings>(
                configuration.GetSection("EmailSettings"));

            return services;
        }
    }
}
