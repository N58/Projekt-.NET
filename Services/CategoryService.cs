using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryRecipeAsync(int categoryId, int recipeId)
        {
            var categoryRecipe = new CategoryRecipe
            {
                CategoryId = categoryId,
                RecipeId = recipeId
            };

            _context.CategoryRecipes.Add(categoryRecipe);
            await _context.SaveChangesAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> FindAsync(int? id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAsync()
        {
            var categories = await _context.Categories
                .ToListAsync();
            return categories;
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(string userId)
        {
            var categories = await _context.Categories
                .Include(c => c.CategoryRecipes)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            return categories;
        }
    }
}
