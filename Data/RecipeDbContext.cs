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
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Likes> Likes { get; set; }

        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Likes>()
                .HasKey(c => new { c.UserId, c.RecipeID });
        }
    }
}
