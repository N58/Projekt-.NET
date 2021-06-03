using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Areas.Identity.Pages.Account
{
    [Authorize]
    public class CategoriesModel : PageModel
    {
        private readonly CategoryService _categoryService;

        public CategoriesModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task OnGetAsync()
        {
            await LoadAsync();
        }

        [BindProperty]
        public Category Category { get; set; }
        public IList<Category> Categories { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Category.UserId = userId;

                // Add to database
                if (await TryUpdateModelAsync<Category>(Category, "category", c => c.Name, c => c.UserId))
                {
                    await _categoryService.AddCategoryAsync(Category);
                    return RedirectToPage("Categories");
                }
                return RedirectToPage("Categories");
            }
            return RedirectToPage("Categories");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (category.UserId == null || category.UserId != userId)
                return RedirectToPage("Categories");

            await _categoryService.RemoveCategoryAsync(category);
            return RedirectToPage("Categories");
        }

        public async Task LoadAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Categories = (IList<Category>)await _categoryService.GetByUserIdAsync(userId);
        }
    }
}
