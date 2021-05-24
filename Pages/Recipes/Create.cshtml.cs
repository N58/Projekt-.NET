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
using PortalKulinarny.Data;
using PortalKulinarny.Models;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly RecipeDbContext _context;

        public CreateModel(RecipeDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Recipe Recipe { get; set; }
        public List<Ingredients> IngredientsList { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Recipe.UserId = userId;
                Recipe.DateTime = DateTime.Now;
                Recipe.ModificationDateTime = DateTime.Now;

                var ingredient = new Ingredients()
                {
                    Name = "test"
                };
                var ingredient2 = new Ingredients()
                {
                    Name = "test2"
                };
                Recipe.Ingredients = new List<Ingredients>();
                Recipe.Ingredients.Add(ingredient);
                Recipe.Ingredients.Add(ingredient2);

                Recipe.Likes = new List<Likes>();

                // Add to database
                if (await TryUpdateModelAsync<Recipe>(Recipe, "recipe", r => r.UserId, r => r.Name, r => r.Description, r => r.DateTime, r => r.ModificationDateTime, r => r.Ingredients, r => r.Likes))
                {
                    _context.Recipe.Add(Recipe);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }

                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }
    }
}
