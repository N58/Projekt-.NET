using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public DatabaseRecipesService RecipeService;
        public IEnumerable<Recipe> Recipes { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, DatabaseRecipesService productService)
        {
            _logger = logger;
            RecipeService = productService;
        }

        public void OnGet()
        {
            Recipes = RecipeService.GetRecipes();
        }
    }
}
