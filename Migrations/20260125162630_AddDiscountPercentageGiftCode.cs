using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountPercentageGiftCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Giftcodes",
                newName: "MaximumDiscountAmount");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercentage",
                table: "Giftcodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Giftcodes");

            migrationBuilder.RenameColumn(
                name: "MaximumDiscountAmount",
                table: "Giftcodes",
                newName: "DiscountAmount");
        }
    }
}
