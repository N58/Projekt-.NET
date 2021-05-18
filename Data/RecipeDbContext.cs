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
        public DbSet<Recipe> Recipes { get; set; }

        public RecipeDbContext(DbContextOptions options) : base(options) { }
    }
}
