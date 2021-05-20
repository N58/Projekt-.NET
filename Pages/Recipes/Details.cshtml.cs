using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;

namespace PortalKulinarny.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly RecipeDbContext _contextRecipes;
        private readonly IngredientsDbContext _contextIngredients;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(RecipeDbContext contextRecipes, IngredientsDbContext contextIngredients, UserManager<ApplicationUser> userManager)
        {
            _contextRecipes = contextRecipes;
            _contextIngredients = contextIngredients;
            _userManager = userManager;
        }

        public Recipe Recipe { get; set; }
        public List<Ingredients> Ingredients { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _contextRecipes.Recipe.FirstOrDefaultAsync(m => m.Id == id);

            if (Recipe == null)
            {
                return NotFound();
            }

            Ingredients = _contextIngredients.Ingredients.Where(m => m.RecipeFK == Recipe.Id).ToList();
            return Page();
        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
