using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UtilsService _utilsService;
        public readonly CategoryService _categoryService;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public Recipe Recipe { get; set; }
        public List<string> Ingredients { get; set; }
        public List<int> CategoriesId { get; set; }
        public List<Category> Categories { get; set; }
        [BindProperty]
        public string NewIngredient { get; set; }
        [BindProperty]
        public int NewCategory { get; set; }
        [BindProperty]
        public Image Image { get; set; }

        private string IngerdientSession = "ingerdientSession";
        private string CategoriesSession = "categoriesSession";

        public CreateModel(ApplicationDbContext context, UtilsService utilsService, CategoryService categoryService, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _utilsService = utilsService;
            _categoryService = categoryService;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult OnGet()
        {
            _utilsService.RemoveSession(HttpContext, CategoriesSession);
            _utilsService.RemoveSession(HttpContext, IngerdientSession);
            return Page();
        }

        

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Recipe.UserId = userId;
                Recipe.DateTime = DateTime.Now;
                Recipe.ModificationDateTime = DateTime.Now;

                // Add to database
                if (await TryUpdateModelAsync<Recipe>(Recipe, "recipe", r => r.UserId, r => r.Name, r => r.Description, r => r.DateTime, r => r.ModificationDateTime))
                {
                    Recipe.Images = new List<Image>();
                    foreach(var file in Recipe.Gallery)
                    {
                        var name = await UploadImage(file);
                        Recipe.Images.Add(new Image { Name = name });
                    }
                    _context.Recipes.Add(Recipe);
                    await AddListsToDb();
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Index");
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            var filenameCombined = Recipe.Name + "_" + fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
            var path = Path.Combine(wwwRootPath + "/Images/", filenameCombined);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return filenameCombined;
        }

        private async Task AddListsToDb()
        {
            await RefreshPageModels();
            Recipe.Ingredients = new List<Ingredient>();
            if (Ingredients != null)
                Ingredients.ForEach(name => Recipe.Ingredients.Add(new Ingredient { Name = name }));
            Recipe.CategoryRecipes = new List<CategoryRecipe>();
            if (Categories != null)
                Categories.ForEach(c => Recipe.CategoryRecipes.Add(new CategoryRecipe { Category = c }));
        }

        public async Task<IActionResult> OnPostAddIngredient()
        {

            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngerdientSession);
            
            
            if (Ingredients == null)
                Ingredients = new List<string>();

            if(!string.IsNullOrWhiteSpace(NewIngredient) && !Ingredients.Contains(NewIngredient))
                Ingredients.Add(NewIngredient);
            
            _utilsService.SetSession(HttpContext, IngerdientSession, Ingredients);

            await RefreshPageModels();
            return Page(); // reloading page without OnGet()
        }

        public async Task<IActionResult> OnPostDeleteIngredient(String name)
        {
            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngerdientSession);

            if (Ingredients == null)
                Ingredients = new List<string>();

            if (Ingredients.Contains(name))
                Ingredients.Remove(name);

            _utilsService.SetSession(HttpContext, IngerdientSession, Ingredients);

            await RefreshPageModels();
            return Page(); // reloading page without OnGet()
        }

        public async Task<IActionResult> OnPostAddCategory()
        {
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (!CategoriesId.Contains(NewCategory))
                CategoriesId.Add(NewCategory);

            _utilsService.SetSession(HttpContext, CategoriesSession, CategoriesId);

            await RefreshPageModels();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteCategory(int id)
        {
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (CategoriesId.Contains(id))
                CategoriesId.Remove(id);

            _utilsService.SetSession(HttpContext, CategoriesSession, CategoriesId);

            await RefreshPageModels();
            return Page(); // reloading page without OnGet()
        }

        public async Task RefreshPageModels()
        {
            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngerdientSession);
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);
            Categories = await _context.Categories
                .Where(c => CategoriesId.Contains(c.Id))
                .ToListAsync();
        }
    }
}
