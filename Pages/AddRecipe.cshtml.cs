using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Data;
using PortalKulinarny.Models;

namespace PortalKulinarny.Pages
{
    [Authorize]
    public class AddRecipeModel : PageModel
    {
        private readonly RecipeDbContext _context;

        [BindProperty]
        public Recipe Recipe { get; set; }

        public AddRecipeModel(RecipeDbContext context)
        {
            _context = context;
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                Recipe.DateTime = DateTime.Now;

                // Add to database
                _context.Recipes.Add(Recipe);
                _context.SaveChanges();

                return RedirectToPage("./Index");
            }

            return Page();
        }

        public void OnGet()
        {

        }
    }
}
