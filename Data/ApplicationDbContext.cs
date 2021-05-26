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

        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.RecipeId });
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Recipe)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.RecipeId);
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.UserId);
        }
    }
}
