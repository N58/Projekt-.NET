using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ImagesService _images;

        public DeleteModel(ApplicationDbContext context, ImagesService images)
        {
            _context = context;
            _images = images;
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Recipes.AsNoTracking().FirstOrDefaultAsync(m => m.RecipeId == id);

            if (Recipe == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
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
            List<Image> toDelete = await _context.Images.Where(i => i.RecipeId == id).ToListAsync();
            foreach(Image image in toDelete)
            {
                _images.DeleteImage(image.Name);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
