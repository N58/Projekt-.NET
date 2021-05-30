using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Services
{
    public class DatabaseRecipesService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseRecipesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe> FindByIdAsync(int? id)
        {
            var Recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Votes)
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            return Recipe;
        }

        public async Task<IEnumerable<Recipe>> FindByUserIdAsync(ApplicationUser user)
        {
            var Recipes = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Votes)
                .Include(r => r.Categories)
                .Where(m => m.UserId == user.Id)
                .ToListAsync();
            return Recipes;
        }

        public async Task Update(Recipe recipe)
        {
            _context.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Recipe recipe)
        {
            _context.Remove(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Votes)
                .Include(r => r.Categories)
                .Include(r => r.User)
                .ToListAsync();
            return recipes;
        }
    }
}
