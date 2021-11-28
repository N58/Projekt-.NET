using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Migrations
{
    public partial class likescomment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modificationDate",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "modificationDate",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
