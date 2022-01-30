using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ads.Migrations
{
    public partial class AdsInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AdsDBContext");

            migrationBuilder.CreateTable(
                name: "Ads",
                schema: "AdsDBContext",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdClicksStats",
                schema: "AdsDBContext",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    AdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdClicksStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdClicksStats_Ads_AdId",
                        column: x => x.AdId,
                        principalSchema: "AdsDBContext",
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdShowStats",
                schema: "AdsDBContext",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    AdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdShowStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdShowStats_Ads_AdId",
                        column: x => x.AdId,
                        principalSchema: "AdsDBContext",
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdClicksStats_AdId",
                schema: "AdsDBContext",
                table: "AdClicksStats",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_AdShowStats_AdId",
                schema: "AdsDBContext",
                table: "AdShowStats",
                column: "AdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdClicksStats",
                schema: "AdsDBContext");

            migrationBuilder.DropTable(
                name: "AdShowStats",
                schema: "AdsDBContext");

            migrationBuilder.DropTable(
                name: "Ads",
                schema: "AdsDBContext");
        }
    }
}
