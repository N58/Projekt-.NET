using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public readonly CategoryService _categoryService;
        private readonly UtilsService _utilsService;
        public readonly DatabaseRecipesService _recipeService;

        public EditModel(ApplicationDbContext context, CategoryService categoryService, UtilsService utilsService, DatabaseRecipesService recipesService)
        {
            _context = context;
            _categoryService = categoryService;
            _utilsService = utilsService;
            _recipeService = recipesService;
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        public List<Category> Categories { get; set; }

        [BindProperty]
        public int CategoryId { get; set; }
        private string categoriesSesionName = "categoriesSesion";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            _utilsService.RemoveSession(HttpContext, categoriesSesionName);

            Recipe = await _context.Recipes
                .Include(r => r.CategoryRecipes)
                .ThenInclude(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            var categories = new List<int>();

            if (id == null)
            {
                return NotFound();
            }

            Recipe.CategoryRecipes.ToList().ForEach(c => categories.Add(c.CategoryId));
            
            _utilsService.SetSession(HttpContext, categoriesSesionName, categories);

            if (Recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Recipe.UserId == null || Recipe.UserId != userId)
                return RedirectToPage("./Index");

            await ReloadAsync(id);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var recipeToUpdate = await _context.Recipes.Include(r => r.CategoryRecipes).FirstOrDefaultAsync(r => r.RecipeId == Recipe.RecipeId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            recipeToUpdate.ModificationDateTime = DateTime.Now;

            if (await TryUpdateModelAsync<Recipe>(recipeToUpdate, "Recipe",
                r => r.Name, r => r.Description, r => r.ModificationDateTime, r => r.CategoryRecipes))
            {
                UpdateCategories(recipeToUpdate);

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Index");
        }

        public void UpdateCategories(Recipe recipeToUpdate)
        {
            var categories = _utilsService.GetSession<List<int>>(HttpContext, categoriesSesionName);
            if(categories != null)
            {
                foreach(var categoryId in categories)
                {
                    if (recipeToUpdate.CategoryRecipes.FirstOrDefault(c => c.CategoryId == categoryId) == null)
                    {
                        recipeToUpdate.CategoryRecipes.Add(new CategoryRecipe { CategoryId = categoryId, RecipeId = recipeToUpdate.RecipeId });
                    }
                }
                foreach (var category in recipeToUpdate.CategoryRecipes)
                {
                    if (!categories.Contains(category.CategoryId))
                    {
                        CategoryRecipe categoryToRemove = recipeToUpdate.CategoryRecipes.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                        _context.CategoryRecipes.Remove(categoryToRemove);
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostAddCategory()
        {
            var categories = _utilsService.GetSession<List<int>>(HttpContext, categoriesSesionName);
            
            if (categories == null)
                categories = new List<int>();

            if (!categories.Contains(CategoryId))
            {
                categories.Add(CategoryId);
            }

            _utilsService.SetSession(HttpContext, categoriesSesionName, categories);

            await ReloadAsync(Recipe.RecipeId);
            return Page(); // reloading page without OnGet()
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var categories = _utilsService.GetSession<List<int>>(HttpContext, categoriesSesionName);

            if (categories == null)
                categories = new List<int>();

            if (categories.Contains(id))
            {
                categories.Remove(id);
            }

            _utilsService.SetSession(HttpContext, categoriesSesionName, categories);

            await ReloadAsync(Recipe.RecipeId);
            return Page(); // reloading page without OnGet()
        }

        public async Task ReloadAsync(int? id)
        {
            var categories = _utilsService.GetSession<List<int>>(HttpContext, categoriesSesionName);

            Recipe = await _context.Recipes
                .Include(r => r.CategoryRecipes)
                .ThenInclude(c => c.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            Categories = await _context.Categories
                .Where(c => categories.Contains(c.Id))
                .ToListAsync();
        }
    }
}
