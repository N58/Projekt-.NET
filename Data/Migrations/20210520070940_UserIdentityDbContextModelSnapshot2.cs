using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Data.Migrations
{
    public partial class UserIdentityDbContextModelSnapshot2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DoJ",
                table: "AspNetUsers",
                type: "DateTime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoJ",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
