using Microsoft.EntityFrameworkCore.Migrations;

namespace LockrFront.Migrations
{
    public partial class Migration_20200109_l : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsValid",
                table: "Domain",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Domain");
        }
    }
}
