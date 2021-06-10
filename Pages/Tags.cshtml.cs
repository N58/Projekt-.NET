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
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages
{
    public class TagsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseRecipesService _recipesService;
        public readonly VoteService _voteService;
        public readonly UserService _userService;
        public readonly FavouritiesService _favouritiesService;
        public readonly CategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TagsModel(ApplicationDbContext context, DatabaseRecipesService recipesService, VoteService voteService, UserService userService,
            FavouritiesService favouritiesService, CategoryService categoryService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _recipesService = recipesService;
            _voteService = voteService;
            _userService = userService;
            _favouritiesService = favouritiesService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public IList<Recipe> Recipes { get; set; }
        public string UserId { get; set; }
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await LoadAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostFavouritiesAsync(int id, int favid)
        {
            var recipeAdded = await _recipesService.FindByIdAsync(favid);

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userAdding = await _context.Users.Include(u => u.Favourites).FirstOrDefaultAsync(u => u.Id == userID);

            try
            {
                await _favouritiesService.AddRemoveFav(recipeAdded, userAdding);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(id);
            return Page();

        }

        public async Task<IActionResult> OnPostUpVoteAsync(int? id, int catid)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.UpVote(recipeVoted, userVoting);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(catid);
            return Page();
        }

        public async Task<IActionResult> OnPostDownVoteAsync(int? id, int catid, string value)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.DownVote(recipeVoted, userVoting);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(catid);
            return Page();
        }

        public async Task LoadAsync(int? id)
        {
            var recipes = from n in await _recipesService.FindByTagAsync(id)
                          select n;

            Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Category = await _categoryService.FindAsync(id);
        }
    }
}
