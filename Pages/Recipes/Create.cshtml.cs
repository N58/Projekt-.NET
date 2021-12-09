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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UtilsService _utilsService;
        public readonly CategoryService _categoryService;
        private readonly ImagesService _imagesService;
        private readonly IHttpContextAccessor _accessor;

        [BindProperty]
        public Recipe Recipe { get; set; }
        [Display(Name = "Dodaj zdjęcie...")]
        public IFormFile NewFile { get; set; }
        public List<string> Ingredients { get; set; }
        public List<int> CategoriesId { get; set; }
        public List<Image> Images { get; set; }
        [BindProperty]
        [Display(Name = "Składniki")]
        public string NewIngredient { get; set; }
        [BindProperty]
        [Display(Name = "Kategorie")]
        public int? NewCategory { get; set; }

        private string IngredientSession = "ingredientSession";
        private string CategoriesSession = "categoriesSession";
        private string ImagesSession = "imagesSession";

        public CreateModel(ApplicationDbContext context, UtilsService utilsService, CategoryService categoryService, ImagesService imagesService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _utilsService = utilsService;
            _categoryService = categoryService;
            _imagesService = imagesService;
            _accessor = contextAccessor;
        }

        public IActionResult OnGet()
        {
            _utilsService.RemoveSession(_accessor.HttpContext, CategoriesSession);
            _utilsService.RemoveSession(_accessor.HttpContext, IngredientSession);
            _utilsService.RemoveSession(_accessor.HttpContext, ImagesSession);
            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Recipe.ViewCount = 0;
                Recipe.UserId = userId;
                Recipe.DateTime = DateTime.Now;
                Recipe.ModificationDateTime = DateTime.Now;
                RefreshPageModels();

                // Add to database
                if (await TryUpdateModelAsync<Recipe>(Recipe, "recipe", r => r.UserId, r => r.Name, r => r.Description, r => r.DateTime, r => r.ModificationDateTime))
                {
                    Recipe.Images = Images;
                    _context.Recipes.Add(Recipe);
                    Images.ForEach(i => i.RecipeId = Recipe.RecipeId);
                    _context.Images.AddRange(Images);
                    AddListsToDb();
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Details", new { id = Recipe.RecipeId});
                }
                return RedirectToPage("./Index");
            }
            return RedirectToPage("./Index");
        }

        public void AddListsToDb()
        {
            Recipe.Ingredients = new List<Ingredient>();
            if (Ingredients != null)
                Ingredients.ForEach(name => Recipe.Ingredients.Add(new Ingredient { Name = name }));
            Recipe.CategoryRecipes = new List<CategoryRecipe>();
            if (CategoriesId != null)
                CategoriesId.ForEach(c => Recipe.CategoryRecipes.Add(new CategoryRecipe { CategoryId = c }));
        }

        public async Task OnPostAddImage()
        {
            Images = _utilsService.GetSession<List<Image>>(_accessor.HttpContext, ImagesSession);

            if (Images == null)
                Images = new List<Image>();

            if(NewFile != null)
            {
                Images.Add(new Image { Name = await _imagesService.UploadImage(NewFile, Recipe.Name) });
            }

            _utilsService.SetSession(_accessor.HttpContext, ImagesSession, Images);

            RefreshPageModels();
        }

        public void OnPostDeleteImage(int index)
        {
            Images = _utilsService.GetSession<List<Image>>(_accessor.HttpContext, ImagesSession);

            if (Images == null)
                Images = new List<Image>();

            if(index < Images.Count)
            {
                if(System.IO.File.Exists(Images[index].GetUrl()))
                {
                    System.IO.File.Delete(Images[index].GetUrl());
                }
                Images.RemoveAt(index);
            }

            _utilsService.SetSession(_accessor.HttpContext, ImagesSession, Images);

            RefreshPageModels();
        }

        public void OnPostAddIngredient()
        {

            Ingredients = _utilsService.GetSession<List<string>>(_accessor.HttpContext, IngredientSession);
            
            
            if (Ingredients == null)
                Ingredients = new List<string>();

            if(!string.IsNullOrWhiteSpace(NewIngredient) && !Ingredients.Contains(NewIngredient))
                Ingredients.Add(NewIngredient);
            
            _utilsService.SetSession(_accessor.HttpContext, IngredientSession, Ingredients);

            RefreshPageModels();
        }

        public void OnPostDeleteIngredient(String name)
        {
            Ingredients = _utilsService.GetSession<List<string>>(_accessor.HttpContext, IngredientSession);

            if (Ingredients == null)
                Ingredients = new List<string>();

            if (Ingredients.Contains(name))
                Ingredients.Remove(name);

            _utilsService.SetSession(_accessor.HttpContext, IngredientSession, Ingredients);

            RefreshPageModels();
        }

        public void OnPostAddCategory()
        {
            CategoriesId = _utilsService.GetSession<List<int>>(_accessor.HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (NewCategory != null && !CategoriesId.Contains((int)NewCategory))
                CategoriesId.Add((int)NewCategory);

            _utilsService.SetSession(_accessor.HttpContext, CategoriesSession, CategoriesId);

            RefreshPageModels();
        }

        public void OnPostDeleteCategory(int id)
        {
            CategoriesId = _utilsService.GetSession<List<int>>(_accessor.HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (CategoriesId.Contains(id))
                CategoriesId.Remove(id);

            _utilsService.SetSession(_accessor.HttpContext, CategoriesSession, CategoriesId);

            RefreshPageModels();
        }

        public void RefreshPageModels()
            {
            Ingredients = _utilsService.GetSession<List<string>>(_accessor.HttpContext, IngredientSession);
            CategoriesId = _utilsService.GetSession<List<int>>(_accessor.HttpContext, CategoriesSession);
            Images = _utilsService.GetSession<List<Image>>(_accessor.HttpContext, ImagesSession);
        }
    }
}
