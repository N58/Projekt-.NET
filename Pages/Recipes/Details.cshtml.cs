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

            Recipe = await _context.Recipe.Include(r => r.Ingredients).Include(r => r.Votes).AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }
        

        public async Task<IActionResult> OnPostLikeDislikeAsync(int? id, string value)
        {
            int valueNormalised;

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var recipeToUpdate = await _context.Recipe.Include(r => r.Ingredients).Include(r => r.Votes).FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipeToUpdate == null)
            {
                return NotFound();
            }

            var userVoting = await _context.Users.FirstOrDefaultAsync(u => u.Id == userID);
            if (userVoting == null || recipeToUpdate.UserId == userID)
            {
                return NotFound();
            }

            if (int.TryParse(value, out valueNormalised))
            {
                if (valueNormalised < 0)
                {
                    valueNormalised = -1;
                }
                if (valueNormalised >= 0)
                {
                    valueNormalised = 1;
                }
            }
            else
            {
                return NotFound();
            }

            var vote = await _context.Votes.FirstOrDefaultAsync(l => l.UserId == userID && l.RecipeId == recipeToUpdate.RecipeId);

            if (vote == null)
            {
                var newVote = new Vote()
                {
                    Recipe = recipeToUpdate, User = userVoting, Value = valueNormalised
                };
                recipeToUpdate.Votes.Add(newVote);
                recipeToUpdate.Rating += valueNormalised;

                if (await TryUpdateModelAsync<Recipe>(recipeToUpdate))
                {
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                recipeToUpdate.Rating -= vote.Value;
                if (await TryUpdateModelAsync<Recipe>(recipeToUpdate))
                {
                    _context.Votes.Remove(vote);
                    await _context.SaveChangesAsync();
                }
            }

            Recipe = await _context.Recipe.Include(r => r.Ingredients).Include(r => r.Votes).AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);
            return Page();

        }


        public async Task<IActionResult> OnPostFavouritiesAsync(int id)
        {

            return Page();

        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
