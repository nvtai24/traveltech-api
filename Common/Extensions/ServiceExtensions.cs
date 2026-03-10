using System.Text.Json.Serialization;
using StackExchange.Redis;
using TravelTechApi.Common.Filters;
using TravelTechApi.Common.Settings;
using TravelTechApi.Services.AI;
using TravelTechApi.Services.Auth;
using TravelTechApi.Services.Cloudinary;
using TravelTechApi.Services.Contact;
using TravelTechApi.Services.Destination;
using TravelTechApi.Services.Email;
using TravelTechApi.Services.Payment;
using TravelTechApi.Services.Plan;
using TravelTechApi.Services.Quota;
using TravelTechApi.Services.UserPlanSubscription;
using TravelTechApi.Services.Giftcode;
using TravelTechApi.Services.WebsiteFeedback;
using TravelTechApi.Services.User;
using TravelTechApi.Services.Audit;
using TravelTechApi.Services.Blog;
using TravelTechApi.Services.BlogComment;
using TravelTechApi.Services.SpinPrize;

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

            services.AddHttpContextAccessor();

            // Register application services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IDestinationService, DestinationService>();
            services.AddScoped<IAIService, OpenAIService>();
            services.AddScoped<IPlanGenerationService, PlanGenerationService>();
            services.AddScoped<IUserPlanSubscriptionService, UserPlanSubscriptionService>();
            services.AddScoped<IQuotaService, QuotaService>();
            services.AddScoped<ITravelHobbyService, TravelHobbyService>();
            services.AddScoped<IPriceSettingService, PriceSettingService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IPaymentService, SepayPaymentService>();
            services.AddScoped<IGiftcodeService, GiftcodeService>();
            services.AddScoped<IWebsiteFeedbackService, WebsiteFeedbackService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogCommentService, Services.BlogComment.BlogCommentService>();
            services.AddScoped<ISpinPrizeService, SpinPrizeService>();

            // Register Hosted Services
            services.AddHostedService<TravelTechApi.Worker.PaymentCleanupWorker>();

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

            services.Configure<SepaySettings>(configuration.GetSection("SepaySettings"));

            // Google Auth Settings
            services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuthSettings"));

            return services;
        }

        /// <summary>
        /// Register logging action filter
        /// </summary>
        public static IServiceCollection AddLoggingFilter(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<LoggingActionFilter>();
            });
            return services;
        }

        public static IServiceCollection AddJsonConverter(this IServiceCollection services)
        {
            services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            return services;
        }

    }
}
