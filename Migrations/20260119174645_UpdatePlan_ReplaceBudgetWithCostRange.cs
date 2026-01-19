using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlan_ReplaceBudgetWithCostRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "Plans",
                newName: "TotalCostEstimatedTo");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCostEstimatedFrom",
                table: "Plans",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCostEstimatedFrom",
                table: "Plans");

            migrationBuilder.RenameColumn(
                name: "TotalCostEstimatedTo",
                table: "Plans",
                newName: "Budget");
        }
    }
}
