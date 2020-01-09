using Microsoft.EntityFrameworkCore.Migrations;

namespace LockrFront.Migrations
{
    public partial class Migration_20200109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domain",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    ExpiryData = table.Column<string>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    NtaMxList = table.Column<string>(nullable: true),
                    SpfVersion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domain", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Domain");
        }
    }
}
