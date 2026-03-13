using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCloudinaryFileInfoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CloudinaryFileInfos_AvatarId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CloudinaryFileInfos");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AvatarId",
                table: "AspNetUsers",
                newName: "AvatarUrl");

            migrationBuilder.AddColumn<List<string>>(
                name: "Images",
                table: "DestinationSharings",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<List<string>>(
                name: "Images",
                table: "Destinations",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "DestinationSharings");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "Destinations");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "AspNetUsers",
                newName: "AvatarId");

            migrationBuilder.CreateTable(
                name: "CloudinaryFileInfos",
                columns: table => new
                {
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: true),
                    DestinationSharingId = table.Column<int>(type: "integer", nullable: true),
                    Format = table.Column<string>(type: "text", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    SecureUrl = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudinaryFileInfos", x => x.PublicId);
                    table.ForeignKey(
                        name: "FK_CloudinaryFileInfos_DestinationSharings_DestinationSharingId",
                        column: x => x.DestinationSharingId,
                        principalTable: "DestinationSharings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CloudinaryFileInfos_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudinaryFileInfos_DestinationId",
                table: "CloudinaryFileInfos",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudinaryFileInfos_DestinationSharingId",
                table: "CloudinaryFileInfos",
                column: "DestinationSharingId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CloudinaryFileInfos_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId",
                principalTable: "CloudinaryFileInfos",
                principalColumn: "PublicId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
