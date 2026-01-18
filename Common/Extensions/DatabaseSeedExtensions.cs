using Microsoft.EntityFrameworkCore;
using TravelTechApi.Data;
using TravelTechApi.Entities;

namespace TravelTechApi.Extensions
{
    /// <summary>
    /// Extension methods for database seeding
    /// </summary>
    public static class DatabaseSeedExtensions
    {
        /// <summary>
        /// Seed initial data for testing
        /// </summary>
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Check if database has any data
                if (await context.Regions.AnyAsync())
                {
                    logger.LogInformation("Database already seeded");
                    return;
                }

                logger.LogInformation("Seeding database with initial data");

                // Seed Regions (3 miền của Việt Nam)
                var regions = new List<Region>
                {
                    new Region { Name = "Miền Bắc" },
                    new Region { Name = "Miền Trung" },
                    new Region { Name = "Miền Nam" }
                };

                await context.Regions.AddRangeAsync(regions);
                await context.SaveChangesAsync();

                // Seed Locations (63 tỉnh thành của Việt Nam)
                var locations = new List<Location>
                {
                    // Miền Bắc (25 tỉnh thành)
                    new Location { Name = "Hà Nội", RegionId = regions[0].Id },
                    new Location { Name = "Hải Phòng", RegionId = regions[0].Id },
                    new Location { Name = "Quảng Ninh", RegionId = regions[0].Id },
                    new Location { Name = "Bắc Ninh", RegionId = regions[0].Id },
                    new Location { Name = "Hải Dương", RegionId = regions[0].Id },
                    new Location { Name = "Hưng Yên", RegionId = regions[0].Id },
                    new Location { Name = "Vĩnh Phúc", RegionId = regions[0].Id },
                    new Location { Name = "Thái Nguyên", RegionId = regions[0].Id },
                    new Location { Name = "Bắc Giang", RegionId = regions[0].Id },
                    new Location { Name = "Lạng Sơn", RegionId = regions[0].Id },
                    new Location { Name = "Cao Bằng", RegionId = regions[0].Id },
                    new Location { Name = "Hà Giang", RegionId = regions[0].Id },
                    new Location { Name = "Tuyên Quang", RegionId = regions[0].Id },
                    new Location { Name = "Phú Thọ", RegionId = regions[0].Id },
                    new Location { Name = "Yên Bái", RegionId = regions[0].Id },
                    new Location { Name = "Lào Cai", RegionId = regions[0].Id },
                    new Location { Name = "Lai Châu", RegionId = regions[0].Id },
                    new Location { Name = "Điện Biên", RegionId = regions[0].Id },
                    new Location { Name = "Sơn La", RegionId = regions[0].Id },
                    new Location { Name = "Hòa Bình", RegionId = regions[0].Id },
                    new Location { Name = "Ninh Bình", RegionId = regions[0].Id },
                    new Location { Name = "Nam Định", RegionId = regions[0].Id },
                    new Location { Name = "Thái Bình", RegionId = regions[0].Id },
                    new Location { Name = "Hà Nam", RegionId = regions[0].Id },
                    new Location { Name = "Bắc Kạn", RegionId = regions[0].Id },
                    
                    // Miền Trung (19 tỉnh thành)
                    new Location { Name = "Thanh Hóa", RegionId = regions[1].Id },
                    new Location { Name = "Nghệ An", RegionId = regions[1].Id },
                    new Location { Name = "Hà Tĩnh", RegionId = regions[1].Id },
                    new Location { Name = "Quảng Bình", RegionId = regions[1].Id },
                    new Location { Name = "Quảng Trị", RegionId = regions[1].Id },
                    new Location { Name = "Thừa Thiên Huế", RegionId = regions[1].Id },
                    new Location { Name = "Đà Nẵng", RegionId = regions[1].Id },
                    new Location { Name = "Quảng Nam", RegionId = regions[1].Id },
                    new Location { Name = "Quảng Ngãi", RegionId = regions[1].Id },
                    new Location { Name = "Bình Định", RegionId = regions[1].Id },
                    new Location { Name = "Phú Yên", RegionId = regions[1].Id },
                    new Location { Name = "Khánh Hòa", RegionId = regions[1].Id },
                    new Location { Name = "Ninh Thuận", RegionId = regions[1].Id },
                    new Location { Name = "Bình Thuận", RegionId = regions[1].Id },
                    new Location { Name = "Kon Tum", RegionId = regions[1].Id },
                    new Location { Name = "Gia Lai", RegionId = regions[1].Id },
                    new Location { Name = "Đắk Lắk", RegionId = regions[1].Id },
                    new Location { Name = "Đắk Nông", RegionId = regions[1].Id },
                    new Location { Name = "Lâm Đồng", RegionId = regions[1].Id },
                    
                    // Miền Nam (19 tỉnh thành)
                    new Location { Name = "TP. Hồ Chí Minh", RegionId = regions[2].Id },
                    new Location { Name = "Bình Dương", RegionId = regions[2].Id },
                    new Location { Name = "Đồng Nai", RegionId = regions[2].Id },
                    new Location { Name = "Bà Rịa - Vũng Tàu", RegionId = regions[2].Id },
                    new Location { Name = "Tây Ninh", RegionId = regions[2].Id },
                    new Location { Name = "Bình Phước", RegionId = regions[2].Id },
                    new Location { Name = "Long An", RegionId = regions[2].Id },
                    new Location { Name = "Tiền Giang", RegionId = regions[2].Id },
                    new Location { Name = "Bến Tre", RegionId = regions[2].Id },
                    new Location { Name = "Vĩnh Long", RegionId = regions[2].Id },
                    new Location { Name = "Trà Vinh", RegionId = regions[2].Id },
                    new Location { Name = "Đồng Tháp", RegionId = regions[2].Id },
                    new Location { Name = "An Giang", RegionId = regions[2].Id },
                    new Location { Name = "Kiên Giang", RegionId = regions[2].Id },
                    new Location { Name = "Cần Thơ", RegionId = regions[2].Id },
                    new Location { Name = "Hậu Giang", RegionId = regions[2].Id },
                    new Location { Name = "Sóc Trăng", RegionId = regions[2].Id },
                    new Location { Name = "Bạc Liêu", RegionId = regions[2].Id },
                    new Location { Name = "Cà Mau", RegionId = regions[2].Id }
                };

                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();

                logger.LogInformation("Database seeded successfully with {RegionCount} regions and {LocationCount} provinces",
                    regions.Count, locations.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
            }
        }
    }
}
