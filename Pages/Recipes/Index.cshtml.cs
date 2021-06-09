using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalKulinarny.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UtilsService _utilsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseRecipesService _recipesService;
        public readonly VoteService _voteService;
        public readonly FavouritiesService _favouritiesService;
        public readonly UserService _userService;
        public readonly CategoryService _categoryService;

        public IndexModel(ApplicationDbContext context,
            UtilsService utilsService, UserManager<ApplicationUser> userManager,
            DatabaseRecipesService recipesService, VoteService voteService, 
            FavouritiesService favouritiesService, UserService userService,
            CategoryService categoryService)
        {
            _context = context;
            _utilsService = utilsService;
            _userManager = userManager;
            _recipesService = recipesService;
            _voteService = voteService;
            _favouritiesService = favouritiesService;
            _userService = userService;
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Szukajka")]
        public string Search { get; set; }
        public IList<Recipe> Recipes { get; set; }
        public string UserId { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        private string SearchSession = "SearchSession";

        public async Task OnGetAsync()
        {
            _utilsService.SetSession(HttpContext, SearchSession, Search);
            await LoadAsync();
        }

        public async Task<IActionResult> OnPostUpVoteAsync(int? id)
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

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDownVoteAsync(int? id, string value)
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

            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostFavouritiesAsync(int id)
        {
            var recipeAdded = await _recipesService.FindByIdAsync(id);

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

            await LoadAsync();
            return Page();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.UserId == null || recipe.UserId != userId)
                return RedirectToPage("./Index");

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                return RedirectToPage("./Index", new { Search = Search });
            }
            await LoadAsync();
            return Page();
        }

        public async Task LoadAsync()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            Search = _utilsService.GetSessionString(HttpContext, SearchSession);

            var recipes = from n in await _recipesService.GetRecipesAsync()
                          select n;

            var users = from u in await _userService.GetUsersAsync()
                        select u;

            var categories = from c in await _categoryService.GetAsync()
                            select c;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                recipes = recipes.Where(s => (culture.CompareInfo.IndexOf(s.Name, Search, CompareOptions.IgnoreCase) >= 0));
                users = users.Where(s => (culture.CompareInfo.IndexOf(s.UserName, Search, CompareOptions.IgnoreCase) >= 0));
                categories = categories.Where(s => (culture.CompareInfo.IndexOf(s.Name, Search, CompareOptions.IgnoreCase) >= 0));
            }

            Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
            Users = users.OrderBy(u => u.UserName).ToList();
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Categories = categories.OrderBy(c => c.Name);

        }
    }
}
