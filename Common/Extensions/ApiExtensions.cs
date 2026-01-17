using Microsoft.OpenApi.Models;
using TravelTechApi.Common.Middleware;

namespace TravelTechApi.Extensions
{
    /// <summary>
    /// Extension methods for API configuration
    /// </summary>
    public static class ApiExtensions
    {
        /// <summary>
        /// Add API controllers and JSON configuration
        /// </summary>
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            // Configure JSON serialization options
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Add global exception handler
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // Add Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}
