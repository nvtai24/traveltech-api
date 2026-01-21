using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameContactTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessage_ContactTopic_ContactTopicId",
                table: "ContactMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactTopic",
                table: "ContactTopic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactMessage",
                table: "ContactMessage");

            migrationBuilder.RenameTable(
                name: "ContactTopic",
                newName: "ContactTopics");

            migrationBuilder.RenameTable(
                name: "ContactMessage",
                newName: "ContactMessages");

            migrationBuilder.RenameIndex(
                name: "IX_ContactMessage_ContactTopicId",
                table: "ContactMessages",
                newName: "IX_ContactMessages_ContactTopicId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ContactMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactTopics",
                table: "ContactTopics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactMessages",
                table: "ContactMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_ContactTopics_ContactTopicId",
                table: "ContactMessages",
                column: "ContactTopicId",
                principalTable: "ContactTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_ContactTopics_ContactTopicId",
                table: "ContactMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactTopics",
                table: "ContactTopics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactMessages",
                table: "ContactMessages");

            migrationBuilder.RenameTable(
                name: "ContactTopics",
                newName: "ContactTopic");

            migrationBuilder.RenameTable(
                name: "ContactMessages",
                newName: "ContactMessage");

            migrationBuilder.RenameIndex(
                name: "IX_ContactMessages_ContactTopicId",
                table: "ContactMessage",
                newName: "IX_ContactMessage_ContactTopicId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ContactMessage",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactTopic",
                table: "ContactTopic",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactMessage",
                table: "ContactMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessage_ContactTopic_ContactTopicId",
                table: "ContactMessage",
                column: "ContactTopicId",
                principalTable: "ContactTopic",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
