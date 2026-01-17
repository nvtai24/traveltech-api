using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;

namespace TravelTechApi.Extensions
{
    /// <summary>
    /// Extension methods for database configuration
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Add database context with PostgreSQL
        /// </summary>
        public static IServiceCollection AddDatabaseConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
