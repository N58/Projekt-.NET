using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UtilsService _utilsService;

        public CreateModel(ApplicationDbContext context, UtilsService utilsService)
        {
            _context = context;
            _utilsService = utilsService;
        }

        public IActionResult OnGet()
        {
            _utilsService.RemoveSession(HttpContext, recipeSessionName); // Clearing session when page is new
            return Page();
        }

        [BindProperty]
        public Recipe Recipe { get; set; }
        [BindProperty]
        public Ingredient NewIngredient { get; set; }
        private string recipeSessionName = "RecipeCreate";

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Recipe = _utilsService.GetSession<Recipe>(HttpContext, recipeSessionName);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Recipe.UserId = userId;
                Recipe.DateTime = DateTime.Now;
                Recipe.ModificationDateTime = DateTime.Now;

                Recipe.Votes = new List<Vote>();

                // Add to database
                if (await TryUpdateModelAsync<Recipe>(Recipe, "recipe", r => r.UserId, r => r.Name, r => r.Description, r => r.DateTime, r => r.ModificationDateTime, r => r.Ingredients, r => r.Votes))
                {
                    _context.Recipes.Add(Recipe);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostAddIngredient()
        {
            Recipe = _utilsService.GetSession<Recipe>(HttpContext, recipeSessionName);
            if (Recipe.Ingredients == null)
                Recipe.Ingredients = new List<Ingredient>();
            Recipe.Ingredients.Add(NewIngredient);
            _utilsService.SetSession(HttpContext, recipeSessionName, Recipe);

            return Page(); // reloading page without OnGet()
        }
    }
}
