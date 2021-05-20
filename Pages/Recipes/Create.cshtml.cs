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
        private readonly RecipeDbContext _contextRecipes;
        private readonly IngredientsDbContext _contextIngredients;

        public CreateModel(RecipeDbContext contextRecipes, IngredientsDbContext contextIngredients)
        {
            _contextRecipes = contextRecipes;
            _contextIngredients = contextIngredients;
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

                // Add to database
                _contextRecipes.Recipe.Add(Recipe);
                await _contextRecipes.SaveChangesAsync();

                var recipeId = Recipe.Id;
                var ingredient = new Ingredients()
                {
                    Name = "test",
                    RecipeFK = recipeId
                };
                var ingredient2 = new Ingredients()
                {
                    Name = "test2",
                    RecipeFK = recipeId
                };
                _contextIngredients.Ingredients.Add(ingredient);
                _contextIngredients.Ingredients.Add(ingredient2);
                await _contextIngredients.SaveChangesAsync();

                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }
    }
}
