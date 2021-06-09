using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Migrations
{
    public partial class imagemodelupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URL",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
