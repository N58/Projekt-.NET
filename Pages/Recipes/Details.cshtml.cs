using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Recipe Recipe { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Recipe.Include(r => r.Ingredients).Include(r => r.Likes).AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var recipeToUpdate = await _context.Recipe.Include(r => r.Ingredients).Include(r => r.Likes).AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userID == null || recipeToUpdate.UserId == userID)
            {
                return NotFound();
            }

            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userID && l.RecipeId == recipeToUpdate.RecipeId);

            Recipe = await _context.Recipe.Include(r => r.Likes).AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);

            if (like == null)
            {
                var newLike = new Like()
                {
                    UserId = userID
                };
                Recipe.Likes.Add(newLike);
                if (await TryUpdateModelAsync<Recipe>(recipeToUpdate, "Recipe", r => r.Likes))
                {
                    await _context.SaveChangesAsync();
                    return Page();
                }
            }
            else
            {

            }
            return Page();

        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
