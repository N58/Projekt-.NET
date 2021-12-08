using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using System;
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
                new SelectListItem {Text = "Alfabetycznie rosnąco", Value = "alphabetical-asc" },
                new SelectListItem {Text = "Alfabetycznie malejąco", Value = "alphabetical-desc" },
                new SelectListItem {Text = "Najlepiej ocenione", Value = "top-rated" },
                new SelectListItem {Text = "Najgorzej ocenione", Value = "worst-rated" },
                new SelectListItem {Text = "Od najnowszego", Value = "newest", Selected = true },
                new SelectListItem {Text = "Od najstarszego", Value = "oldest" },
                new SelectListItem {Text = "Najczęściej komentowane", Value = "most-commented" },
                new SelectListItem {Text = "Najradziej komentowane", Value = "least-commented" }
            };

            SortUsersModes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Alfabetycznie rosnąco", Value = "alphabetical-asc" },
                new SelectListItem {Text = "Alfabetycznie malejąco", Value = "alphabetical-desc" },
                new SelectListItem {Text = "Od najnowszego", Value = "newest", Selected = true },
                new SelectListItem {Text = "Od najstarszego", Value = "oldest" },
                new SelectListItem {Text = "Po liczbie dodanych przepisów malejąco", Value = "most-recipes" },
                new SelectListItem {Text = "Po liczbie dodanych przepisów rosnąco", Value = "least-recipes" }
            };

            SortCategoriesModes = new List<SelectListItem>
            {
                new SelectListItem {Text = "Alfabetycznie rosnąco", Value = "alphabetical-asc" },
                new SelectListItem {Text = "Alfabetycznie malejąco", Value = "alphabetical-desc" },
            };

            FilterUserNames = new List<SelectListItem>
            {
                new SelectListItem {Text = "Wszyscy użytkownicy", Value = "-1", Selected = true }
            };
            FilterCategories = new List<SelectListItem>
            {
                new SelectListItem {Text = "Wszystkie kategorie", Value = "-1", Selected = true }
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
        [BindProperty(SupportsGet = true)]
        public string FilterUserName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string FilterCategory { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Clear { get; set; }



        public IList<Recipe> Recipes { get; set; }
        public string UserId { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public List<SelectListItem> SortRecipesModes { get; set; }
        public List<SelectListItem> SortUsersModes { get; set; }
        public List<SelectListItem> SortCategoriesModes { get; set; }
        public List<SelectListItem> FilterUserNames { get; set; }
        public List<SelectListItem> FilterCategories { get; set; }

        private string SearchSession = "SearchSession";
        private string SortRecipesSession = "SortRecipesSession";
        private string SortUsersSession = "SortUsersSession";
        private string SortCategoriesSession = "SortCategoriesSession";
        private string FilterCategorySession = "FilterCategory";
        private string FilterUserSession = "FilterUserSession";

        public async Task OnGetAsync()
        {
            if(Clear == "true")
            {
                _utilsService.SetSession(HttpContext, SearchSession, Search);
                _utilsService.SetSession(HttpContext, SortRecipesSession, SortRecipes);
                _utilsService.SetSession(HttpContext, SortUsersSession, SortUsers);
                _utilsService.SetSession(HttpContext, SortCategoriesSession, SortCategories);
                _utilsService.SetSession(HttpContext, FilterUserSession, FilterUserName);
                _utilsService.SetSession(HttpContext, FilterCategorySession, FilterCategory);
            }
            
            if(!string.IsNullOrWhiteSpace(Search))
                _utilsService.SetSession(HttpContext, SearchSession, Search);

            if(!string.IsNullOrWhiteSpace(SortRecipes))
                _utilsService.SetSession(HttpContext, SortRecipesSession, SortRecipes);

            if (!string.IsNullOrWhiteSpace(SortUsers))
                _utilsService.SetSession(HttpContext, SortUsersSession, SortUsers);

            if (!string.IsNullOrWhiteSpace(SortCategories))
                _utilsService.SetSession(HttpContext, SortCategoriesSession, SortCategories);

            if (!string.IsNullOrWhiteSpace(FilterUserName))
                _utilsService.SetSession(HttpContext, FilterUserSession, FilterUserName);

            if (!string.IsNullOrWhiteSpace(FilterCategory))
                _utilsService.SetSession(HttpContext, FilterCategorySession, FilterCategory);

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
            FilterUserName = _utilsService.GetSessionString(HttpContext, FilterUserSession);
            FilterCategory = _utilsService.GetSessionString(HttpContext, FilterCategorySession);

            var recipes = from n in await _recipesService.GetRecipesAsync()
                          select n;

            var users = from u in await _userService.GetUsersAsync()
                        select u;

            var categories = from c in await _categoryService.GetAsync()
                            select c;

            var categoryRecipes = await _context.CategoryRecipes.ToListAsync();

            var comments = await _context.Comments.ToListAsync();

            foreach (var user in users)
            {
                FilterUserNames.Add(new SelectListItem(user.UserName, user.Id));
            }

            foreach (var category in categories)
            {
                FilterCategories.Add(new SelectListItem(category.Name, category.Id.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                recipes = recipes.Where(s => (culture.CompareInfo.IndexOf(s.Name, Search, CompareOptions.IgnoreCase) >= 0));
                users = users.Where(s => (culture.CompareInfo.IndexOf(s.UserName, Search, CompareOptions.IgnoreCase) >= 0));
                categories = categories.Where(s => (culture.CompareInfo.IndexOf(s.Name, Search, CompareOptions.IgnoreCase) >= 0));
            }

            if (!string.IsNullOrWhiteSpace(FilterCategory) && !FilterCategory.Equals("-1"))
            {
                recipes = recipes.Where(r => categoryRecipes.Where(cr => cr.CategoryId == Int32.Parse(FilterCategory)).Any(cr => cr.RecipeId == r.RecipeId));
            }

            if (!string.IsNullOrWhiteSpace(FilterUserName) && !FilterUserName.Equals("-1"))
            {
                recipes = recipes.Where(r => r.UserId == FilterUserName);
            }

            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            switch (SortRecipes)
            {
                case "alphabetical-asc":
                    Recipes = recipes.OrderBy(r => r.Name).ToList();
                    break;
                case "alphabetical-desc":
                    Recipes = recipes.OrderByDescending(r => r.Name).ToList();
                    break;
                case "top-rated":
                    Recipes = recipes.OrderByDescending(r => r.Rating).ToList();
                    break;
                case "worst-rated":
                    Recipes = recipes.OrderBy(r => r.Rating).ToList();
                    break;
                case "newest":
                    Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
                    break;
                case "oldest":
                    Recipes = recipes.OrderBy(r => r.DateTime).ToList();
                    break;
                case "most-commented":
                    Recipes = recipes.OrderByDescending(r => comments.Where(c => c.RecipeId == r.RecipeId).Count()).ToList();
                    break;
                case "least-commented":
                    Recipes = recipes.OrderBy(r => comments.Where(c => c.RecipeId == r.RecipeId).Count()).ToList();
                    break;
                default:
                    Recipes = recipes.OrderByDescending(r => r.DateTime).ToList();
                    break;
            }

            switch (SortUsers)
            {
                case "alphabetical-asc":
                    Users = users.OrderBy(u => u.UserName).ToList();
                    break;
                case "alphabetical-desc":
                    Users = users.OrderByDescending(u => u.UserName).ToList();
                    break;
                case "newest":
                    Users = users.OrderByDescending(u => u.DoJ).ToList();
                    break;
                case "oldest":
                    Users = users.OrderBy(u => u.DoJ).ToList();
                    break;
                case "most-recipes":
                    Users = users.OrderByDescending(u => recipes.Where(r => r.UserId == u.Id).Count()).ToList();
                    break;
                case "least-recipes":
                    Users = users.OrderBy(u => recipes.Where(r => r.UserId == u.Id).Count()).ToList();
                    break;

                default:
                    Users = users.OrderByDescending(u => u.DoJ).ToList();
                    break;
            }

            switch (SortCategories)
            {
                case "alphabetical-asc":
                    Categories = categories.OrderBy(c => c.Name);
                    break;
                case "alphabetical-desc":
                    Categories = categories.OrderByDescending(c => c.Name);
                    break;
                default:
                    Categories = categories.OrderBy(c => c.Name);
                    break;
            }
        }
    }
}
