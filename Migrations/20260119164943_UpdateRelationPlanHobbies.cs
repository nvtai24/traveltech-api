using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationPlanHobbies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Destinations_DestinationId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Plans_PriceSetting_PriceSettingId",
                table: "Plans");

            migrationBuilder.DropForeignKey(
                name: "FK_TravelHobby_Plans_PlanId",
                table: "TravelHobby");

            migrationBuilder.DropIndex(
                name: "IX_Activities_DestinationId",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelHobby",
                table: "TravelHobby");

            migrationBuilder.DropIndex(
                name: "IX_TravelHobby_PlanId",
                table: "TravelHobby");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceSetting",
                table: "PriceSetting");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "TravelHobby");

            migrationBuilder.RenameTable(
                name: "TravelHobby",
                newName: "TravelHobbies");

            migrationBuilder.RenameTable(
                name: "PriceSetting",
                newName: "PriceSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TravelHobbies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelHobbies",
                table: "TravelHobbies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceSettings",
                table: "PriceSettings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PlanTravelHobby",
                columns: table => new
                {
                    HobbiesId = table.Column<int>(type: "integer", nullable: false),
                    PlansId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTravelHobby", x => new { x.HobbiesId, x.PlansId });
                    table.ForeignKey(
                        name: "FK_PlanTravelHobby_Plans_PlansId",
                        column: x => x.PlansId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanTravelHobby_TravelHobbies_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "TravelHobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanTravelHobby_PlansId",
                table: "PlanTravelHobby",
                column: "PlansId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_PriceSettings_PriceSettingId",
                table: "Plans",
                column: "PriceSettingId",
                principalTable: "PriceSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_PriceSettings_PriceSettingId",
                table: "Plans");

            migrationBuilder.DropTable(
                name: "PlanTravelHobby");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelHobbies",
                table: "TravelHobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceSettings",
                table: "PriceSettings");

            migrationBuilder.RenameTable(
                name: "TravelHobbies",
                newName: "TravelHobby");

            migrationBuilder.RenameTable(
                name: "PriceSettings",
                newName: "PriceSetting");

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "Activities",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TravelHobby",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "TravelHobby",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelHobby",
                table: "TravelHobby",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceSetting",
                table: "PriceSetting",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DestinationId",
                table: "Activities",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelHobby_PlanId",
                table: "TravelHobby",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Destinations_DestinationId",
                table: "Activities",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_PriceSetting_PriceSettingId",
                table: "Plans",
                column: "PriceSettingId",
                principalTable: "PriceSetting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TravelHobby_Plans_PlanId",
                table: "TravelHobby",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }
    }
}
