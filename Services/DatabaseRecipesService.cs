using PortalKulinarny.Data;
using PortalKulinarny.Models;
using System.Collections.Generic;

namespace PortalKulinarny.Services
{
    public class DatabaseRecipesService
    {
        private readonly RecipeDbContext _context;

        public DatabaseRecipesService(RecipeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _context.Recipe;
        }
    }
}
