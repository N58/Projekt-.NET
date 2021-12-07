using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            SortRecipesModes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Od najnowszego", Value = "newest", Selected = true },
                new SelectListItem {Text = "Od najstarszego", Value = "oldest" },
                new SelectListItem {Text = "Alfabetycznie", Value = "alphabetical" },
            };

            SortUsersModes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Od najnowszego", Value = "newest", Selected = true },
                new SelectListItem {Text = "Od najstarszego", Value = "oldest" },
                new SelectListItem {Text = "Alfabetycznie", Value = "alphabetical" },
            };

            SortCategoriesModes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Alfabetycznie", Value = "alphabetical", Selected = true }
            };
        }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Szukajka")]
        public string Search { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortRecipes { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortUsers { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortCategories { get; set; }


        public IList<Recipe> Recipes { get; set; }
        public string UserId { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public List<SelectListItem> SortRecipesModes { get; set; }
        public List<SelectListItem> SortUsersModes { get; set; }
        public List<SelectListItem> SortCategoriesModes { get; set; }

        private string SearchSession = "SearchSession";
        private string SortRecipesSession = "SortRecipesSession";
        private string SortUsersSession = "SortUsersSession";
        private string SortCategoriesSession = "SortCategoriesSession";

        public async Task OnGetAsync()
        {
            if(!string.IsNullOrWhiteSpace(Search))
                _utilsService.SetSession(HttpContext, SearchSession, Search);

            if(!string.IsNullOrWhiteSpace(SortRecipes))
                _utilsService.SetSession(HttpContext, SortRecipesSession, SortRecipes);

            if (!string.IsNullOrWhiteSpace(SortUsers))
                _utilsService.SetSession(HttpContext, SortUsersSession, SortUsers);

            if (!string.IsNullOrWhiteSpace(SortCategories))
                _utilsService.SetSession(HttpContext, SortCategoriesSession, SortCategories);

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
                return RedirectToPage("./Index", "Search", new { Search = Search });
            }
            await LoadAsync();
            return Page();
        }

        public async Task LoadAsync()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;

            Search = _utilsService.GetSessionString(HttpContext, SearchSession);
            SortRecipes = _utilsService.GetSessionString(HttpContext, SortRecipesSession);
            SortUsers = _utilsService.GetSessionString(HttpContext, SortUsersSession);
            SortCategories = _utilsService.GetSessionString(HttpContext, SortCategoriesSession);

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

            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            switch (SortRecipes)
            {
                case "newest":
                    Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
                    break;
                case "oldest":
                    Recipes = recipes.OrderBy(r => r.DateTime).ToList();
                    break;
                case "alphabetical":
                    Recipes = recipes.OrderBy(r => r.Name).ToList();
                    break;
                default:
                    Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
                    break;
            }

            switch (SortUsers)
            {
                case "newest":
                    Users = users.OrderByDescending(u => u.DoJ).ToList();
                    break;
                case "oldest":
                    Users = users.OrderBy(u => u.DoJ).ToList();
                    break;
                case "alphabetical":
                    Users = users.OrderBy(u => u.UserName).ToList();
                    break;
                default:
                    Users = users.OrderByDescending(u => u.DoJ).ToList();
                    break;
            }

            switch (SortCategories)
            {
                case "alphabetical":
                    Categories = categories.OrderBy(c => c.Name);
                    break;
                default:
                    Categories = categories.OrderBy(c => c.Name);
                    break;
            }
        }
    }
}
