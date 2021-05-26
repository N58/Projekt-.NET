using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Data
{
    public class RecipeDbContext : DbContext
    {
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Like> Likes { get; set; }

        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
