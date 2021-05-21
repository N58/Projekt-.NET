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
        [Display(Name = "Szukajka")]
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }
        public DatabaseRecipesService RecipeService;
        public IEnumerable<Recipe> Recipes { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, DatabaseRecipesService productService)
        {
            _logger = logger;
            RecipeService = productService;
        }
        public IActionResult OnPost()
        {
            if (!String.IsNullOrWhiteSpace(Search))
            {
                return RedirectToPage("./Recipes/Index/", new { Search = Search });
            }
            return Page();
        }

        public void OnGet()
        {
            Recipes = RecipeService.GetRecipes();
        }
    }
}
