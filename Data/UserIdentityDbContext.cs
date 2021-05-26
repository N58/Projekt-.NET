using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using PortalKulinarny.Areas.Identity.Data;
using System.Text;
using PortalKulinarny.Models;

namespace PortalKulinarny.Data
{
    public class UserIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options)
            : base(options)
        {
        }

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
