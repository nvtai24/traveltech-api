using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBankCodeFromPaymentTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "PaymentTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);
        }
    }
}
