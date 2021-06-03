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
        
        [BindProperty]
        public int CategoryId { get; set; }
        private string recipeSessionName = "RecipeEdit";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            _utilsService.RemoveSession(HttpContext, recipeSessionName);

            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Recipes
                .Include(r => r.CategoryRecipes)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(r => r.RecipeId == id);
            _utilsService.SetSession(HttpContext, recipeSessionName, Recipe);

            if (Recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Recipe.UserId == null || Recipe.UserId != userId)
                return RedirectToPage("./Index");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var recipe = _utilsService.GetSession<Recipe>(HttpContext, recipeSessionName);

            var recipeToUpdate = await _context.Recipes.FindAsync(Recipe.RecipeId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            recipeToUpdate.ModificationDateTime = DateTime.Now;
            recipeToUpdate.CategoryRecipes = recipe.CategoryRecipes;

            if (await TryUpdateModelAsync(recipeToUpdate, "Recipe",
                r => r.Name, r => r.Description, r => r.ModificationDateTime, r => r.CategoryRecipes))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddCategoryAsync()
        {
            Recipe = _utilsService.GetSession<Recipe>(HttpContext, recipeSessionName);
            var Category = await _categoryService.FindAsync(CategoryId);
            var cat = new CategoryRecipe()
            {
                CategoryId = Category.Id,
                Category = Category,
                RecipeId = Recipe.RecipeId,
                Recipe = Recipe
            };

            if (Recipe.CategoryRecipes == null)
                Recipe.CategoryRecipes = new List<CategoryRecipe>();

            if (Recipe.CategoryRecipes.FirstOrDefault(c => c.CategoryId == cat.CategoryId) == null)
                Recipe.CategoryRecipes.Add(cat);

            _utilsService.SetSession(HttpContext, recipeSessionName, Recipe);

            return Page(); // reloading page without OnGet()
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            Recipe = _utilsService.GetSession<Recipe>(HttpContext, recipeSessionName);

            var cat = Recipe.CategoryRecipes.FirstOrDefault(c => c.CategoryId == id);
            if (cat != null)
            {
                Recipe.CategoryRecipes.Remove(cat);
                _utilsService.SetSession(HttpContext, recipeSessionName, Recipe);
            }

            return Page(); // reloading page without OnGet()
        }
    }
}
