using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        [Required(ErrorMessage = "Pusto >:(")]
        [Display(Name = "Szukajka")]
        [BindProperty]
        public string Search { get; set; }
        public DatabaseRecipesService _recipeService;
        public IEnumerable<Recipe> Recipes { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, DatabaseRecipesService recipeService)
        {
            _logger = logger;
            _recipeService = recipeService;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                return RedirectToPage("./Recipes/Index", new { Search = Search });
            }
            Recipes = await GetRecipesAsync();
            return Page();
        }

        public async Task OnGetAsync()
        {
            Recipes = await GetRecipesAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            var recipes = await _recipeService.GetRecipesAsync();
            return recipes.OrderByDescending(r => r.Rating).Take(10);
        }
    }
}
