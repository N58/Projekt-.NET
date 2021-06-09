using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages
{
    public class TagsModel : PageModel
    {
        private readonly DatabaseRecipesService _recipesService;
        public readonly VoteService _voteService;
        public readonly UserService _userService;
        public readonly FavouritiesService _favouritiesService;
        public readonly CategoryService _categoryService;

        public TagsModel(DatabaseRecipesService recipesService, VoteService voteService, UserService userService,
            FavouritiesService favouritiesService, CategoryService categoryService)
        {
            _recipesService = recipesService;
            _voteService = voteService;
            _userService = userService;
            _favouritiesService = favouritiesService;
            _categoryService = categoryService;
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
