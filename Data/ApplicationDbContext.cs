using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using PortalKulinarny.Areas.Identity.Data;
using System.Text;
using PortalKulinarny.Models;

namespace PortalKulinarny.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryRecipe> CategoryRecipes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentsLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Recipe>(u => u.Recipes)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasKey(v => new { v.UserId, v.RecipeId });
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Recipe)
                .WithMany(v => v.Votes)
                .HasForeignKey(v => v.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(v => v.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favourite>()
                .HasKey(f => new { f.UserId, f.RecipeId });
            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.Recipe)
                .WithMany(f => f.Favourites)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany(f => f.Favourites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryRecipe>()
                .HasKey(c => new { c.CategoryId, c.RecipeId });
            modelBuilder.Entity<CategoryRecipe>()
                .HasOne(c => c.Category)
                .WithMany(c => c.CategoryRecipes)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CategoryRecipe>()
                .HasOne(c => c.Recipe)
                .WithMany(c => c.CategoryRecipes)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentLike>()
                .HasKey(e => new { e.UserId, e.CommentId });
            modelBuilder.Entity<CommentLike>()
                .HasOne<ApplicationUser>(e => e.user)
                .WithMany(e => e.commentsLikes)
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CommentLike>()
                .HasOne<Comment>(e => e.comment)
                .WithMany(e => e.commentsLikes)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Comments)
                .WithOne(e => e.user)
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.Comments)
                .WithOne(e => e.recipe)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
