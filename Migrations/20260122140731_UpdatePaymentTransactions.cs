using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PaymentTransactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "PaymentTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
