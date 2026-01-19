using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPlanStatusToIsSaved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Plans");

            migrationBuilder.AddColumn<bool>(
                name: "IsSaved",
                table: "Plans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSaved",
                table: "Plans");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Plans",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Draft");
        }
    }
}
