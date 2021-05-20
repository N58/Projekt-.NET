using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Data
{
    public class IngredientsDbContext : DbContext
    {
        public DbSet<Ingredients> Ingredients { get; set; }

        public IngredientsDbContext(DbContextOptions options) : base(options) { }
    }
}
