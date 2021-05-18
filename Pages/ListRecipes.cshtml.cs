using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages
{
    public class ListRecipesModel : PageModel
    {
        public DatabaseRecipesService RecipeService;
        public IList<Recipe> Recipes { get; private set; }

        public ListRecipesModel(DatabaseRecipesService productService)
        {
            RecipeService = productService;
        }

        public void OnGet()
        {
            Recipes = (IList<Recipe>)RecipeService.GetRecipes();
        }
    }
}
