using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftcodeToPaymentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GiftcodeId",
                table: "PaymentTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalAmount",
                table: "PaymentTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_GiftcodeId",
                table: "PaymentTransactions",
                column: "GiftcodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Giftcodes_GiftcodeId",
                table: "PaymentTransactions",
                column: "GiftcodeId",
                principalTable: "Giftcodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Giftcodes_GiftcodeId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_GiftcodeId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "GiftcodeId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                table: "PaymentTransactions");
        }
    }
}
