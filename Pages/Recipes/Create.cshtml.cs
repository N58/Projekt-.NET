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

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            RemoveSession(recipeSessionName); // Clearing session when page is new
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
                Recipe = GetSession<Recipe>(recipeSessionName);
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
                    RemoveSession(recipeSessionName);
                    return RedirectToPage("./Index");
                }

                RemoveSession(recipeSessionName);
                return RedirectToPage("./Index");
            }

            RemoveSession(recipeSessionName);
            return RedirectToPage("./Index");
        }

        public IActionResult OnPostAddIngredient()
        {
            Recipe = GetSession<Recipe>(recipeSessionName);
            if (Recipe.Ingredients == null)
                Recipe.Ingredients = new List<Ingredient>();
            Recipe.Ingredients.Add(NewIngredient);
            SetSession(recipeSessionName, Recipe);

            return Page(); // reloading page without OnGet()
        }

        private T GetSession<T>(string name) where T : new()
        {
            var jsonSession = HttpContext.Session.GetString(name);
            if (jsonSession != null)
                return JsonConvert.DeserializeObject<T>(jsonSession);
            else
                return new T();
        }

        private void SetSession(string name, object obj)
        {
            HttpContext.Session.SetString(name, JsonConvert.SerializeObject(obj));
        }

        private void RemoveSession(string name)
        {
            HttpContext.Session.Remove(name);
        }
    }
}
