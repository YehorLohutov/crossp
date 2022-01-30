using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Files.Migrations
{
    public partial class FilesInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FilesDatabaseContext");

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "FilesDatabaseContext",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "FilesDatabaseContext",
                table: "Files",
                columns: new[] { "Id", "Extension", "ExternalId", "Name", "Path", "UserExternalId" },
                values: new object[] { 1, ".mp4", "724df22d-0e4e-4fd1-9e0e-e2cc5cfd8b8d", "Default mp4", "/default.mp4", "f07af1d1-7b23-445a-ae46-a418799ef6ba" });

            migrationBuilder.InsertData(
                schema: "FilesDatabaseContext",
                table: "Files",
                columns: new[] { "Id", "Extension", "ExternalId", "Name", "Path", "UserExternalId" },
                values: new object[] { 2, ".png", "df10063b-7486-4fd1-8ded-acf96cd2dd1f", "Default png", "/default.png", "f07af1d1-7b23-445a-ae46-a418799ef6ba" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "FilesDatabaseContext");
        }
    }
}
