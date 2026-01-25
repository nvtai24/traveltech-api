using TravelTechApi.Common.Extensions;

namespace TravelTechApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddConfigurations(builder.Configuration);
            builder.Services.AddDatabaseConfiguration(builder.Configuration);
            builder.Services.AddIdentityConfiguration();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddCorsConfiguration(builder.Configuration);
            builder.Services.AddApiConfiguration();
            builder.Services.AddLoggingFilter();
            builder.Services.AddJsonConverter();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("DefaultCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            // For case: app behind reverse proxy/load balancer (Nginx, ...)
            // app.UseForwardedHeaders(new ForwardedHeadersOptions
            // {
            //     ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
            //                        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            // });


            // Custom Middlewares
            // app.UseMiddleware<TravelTechApi.Common.Middlewares.RateLimitMiddleware>();

            app.MapControllers();

            // Seed roles and database with initial data
            await app.SeedRolesAsync();
            await app.SeedDestinationDataAsync();

            app.Run();
        }
    }
}
