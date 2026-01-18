using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationAppUserSharing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DestinationSharings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Destinations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationSharings_UserId",
                table: "DestinationSharings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DestinationSharings_AspNetUsers_UserId",
                table: "DestinationSharings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DestinationSharings_AspNetUsers_UserId",
                table: "DestinationSharings");

            migrationBuilder.DropIndex(
                name: "IX_DestinationSharings_UserId",
                table: "DestinationSharings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DestinationSharings");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Destinations");
        }
    }
}
