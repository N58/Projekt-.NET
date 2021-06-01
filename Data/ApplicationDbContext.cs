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
                .WithMany(v => v.Likes)
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


            modelBuilder.Entity<Category>()
                .HasKey(c => new { c.UserId, c.RecipeId });
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Recipe)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(c => c.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
