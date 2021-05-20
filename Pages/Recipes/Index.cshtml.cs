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
    public class IndexModel : PageModel
    {
        private readonly RecipeDbContext _contextRecipes;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(RecipeDbContext contextRecipes, UserManager<ApplicationUser> userManager)
        {
            _contextRecipes = contextRecipes;
            _userManager = userManager;
        }

        public IList<Recipe> Recipe { get; set; }

        public async Task OnGetAsync()
        {
            Recipe = await _contextRecipes.Recipe.ToListAsync();
        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
