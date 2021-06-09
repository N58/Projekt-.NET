using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalKulinarny.Migrations
{
    public partial class fssd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRecipes_Recipes_RecipeId",
                table: "CategoryRecipes");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRecipes_Recipes_RecipeId",
                table: "CategoryRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRecipes_Recipes_RecipeId",
                table: "CategoryRecipes");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRecipes_Recipes_RecipeId",
                table: "CategoryRecipes",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
