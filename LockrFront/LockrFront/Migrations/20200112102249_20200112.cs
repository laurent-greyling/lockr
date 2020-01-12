using Microsoft.EntityFrameworkCore.Migrations;

namespace LockrFront.Migrations
{
    public partial class _20200112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Domain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Domain",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
