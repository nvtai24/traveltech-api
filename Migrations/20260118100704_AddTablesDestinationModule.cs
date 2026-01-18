using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelTechApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesDestinationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RegionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    History = table.Column<string>(type: "text", nullable: false),
                    Lat = table.Column<decimal>(type: "numeric", nullable: false),
                    Lon = table.Column<decimal>(type: "numeric", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DestinationSharings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationSharings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinationSharings_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQs_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloudinaryFileInfos",
                columns: table => new
                {
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    SecureUrl = table.Column<string>(type: "text", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: true),
                    DestinationSharingId = table.Column<int>(type: "integer", nullable: true)
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
                name: "IX_CloudinaryFileInfos_DestinationId",
                table: "CloudinaryFileInfos",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudinaryFileInfos_DestinationSharingId",
                table: "CloudinaryFileInfos",
                column: "DestinationSharingId");

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_LocationId",
                table: "Destinations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationSharings_DestinationId",
                table: "DestinationSharings",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_DestinationId",
                table: "FAQs",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_RegionId",
                table: "Locations",
                column: "RegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudinaryFileInfos");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "DestinationSharings");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
