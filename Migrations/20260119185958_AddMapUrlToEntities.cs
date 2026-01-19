using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMapUrlToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "FoodRecommendations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "Activities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapUrl",
                table: "AccommodationRecommendations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "FoodRecommendations");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "MapUrl",
                table: "AccommodationRecommendations");
        }
    }
}
