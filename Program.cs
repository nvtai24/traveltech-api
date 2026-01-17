using TravelTechApi.Extensions;

namespace TravelTechApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddDatabaseConfiguration(builder.Configuration);
            builder.Services.AddIdentityConfiguration();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddApiConfiguration();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
