using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Migrations
{
    public partial class relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsLikes_AspNetUsers_UserId",
                table: "CommentsLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
