using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Migrations
{
    public partial class fixedRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Recipes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
