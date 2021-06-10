using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using PortalKulinarny.Data;

namespace PortalKulinarny.Areas.Identity.Pages.Account
{
    public class EditCategory : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Category Category { get; set; }
        public CategoryService _categoryService { get; }

        public EditCategory(CategoryService categoryService, ApplicationDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _categoryService.FindAsync(id);

            if (Category == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Category.UserId == null || Category.UserId != userId)
                return RedirectToPage("./Categories");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (ModelState.IsValid)
            {
                var categoryToUpdate = await _categoryService.FindAsync(id);

                if (categoryToUpdate == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (categoryToUpdate.UserId == null || categoryToUpdate.UserId != userId)
                    return RedirectToPage("./Categories");



                if (await TryUpdateModelAsync<Category>(categoryToUpdate, "Category",
                    c => c.Name))
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Categories");
                }
                return RedirectToPage("./Categories");
            }
            else return Page();
        }
    }
}
