using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.ComponentModel;

namespace PortalKulinarny.Pages.Recipes
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public readonly CategoryService _categoryService;
        private readonly UtilsService _utilsService;
        public readonly DatabaseRecipesService _recipeService;
        private readonly ImagesService _imagesService;

        public EditModel(ApplicationDbContext context, CategoryService categoryService, UtilsService utilsService,
            DatabaseRecipesService recipesService, ImagesService imagesService)
        {
            _context = context;
            _categoryService = categoryService;
            _utilsService = utilsService;
            _recipeService = recipesService;
            _imagesService = imagesService;
        }

        [BindProperty]
        public Recipe Recipe { get; set; }
        public List<string> Ingredients { get; set; }
        public List<int> CategoriesId { get; set; }
        public List<Image> Images { get; set; }
        [BindProperty]
        [Display(Name = "Składniki")]
        public string NewIngredient { get; set; }
        [BindProperty]
        [Display(Name = "Kategorie")]
        public int? NewCategory { get; set; }
        [BindProperty, Display(Name = "Wybierz zdjęcie...")]
        public IFormFile NewImage { get; set; }

        private string IngredientsSession = "ingredientSession";
        private string CategoriesSession = "categoriesSession";
        private string ImagesSession = "imagesSession";
        private string RecipeIdSession = "recipeId";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            _utilsService.RemoveSession(HttpContext, IngredientsSession);
            _utilsService.RemoveSession(HttpContext, CategoriesSession);
            _utilsService.RemoveSession(HttpContext, ImagesSession);
            _utilsService.RemoveSession(HttpContext, RecipeIdSession);

            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _recipeService.FindByIdAsync(id);

            if (Recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Recipe.UserId == null || Recipe.UserId != userId)
                return RedirectToPage("./Index");

            CategoriesId = new List<int>();
            Recipe.CategoryRecipes.ToList().ForEach(c => CategoriesId.Add(c.CategoryId));

            Ingredients = new List<string>();
            Recipe.Ingredients.ToList().ForEach(i => Ingredients.Add(i.Name));

            Images = new List<Image>(Recipe.Images);

            var recipeId = Recipe.RecipeId;

            _utilsService.SetSession(HttpContext, CategoriesSession, CategoriesId);
            _utilsService.SetSession(HttpContext, ImagesSession, Images);
            _utilsService.SetSession(HttpContext, IngredientsSession, Ingredients);
            _utilsService.SetSession(HttpContext, RecipeIdSession, recipeId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var recipeId = _utilsService.GetSession<int>(HttpContext, RecipeIdSession);

            var recipeToUpdate = await _recipeService.FindByIdAsync(recipeId);

            if (recipeToUpdate == null)
            {
                return NotFound();
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipeToUpdate.UserId == null || recipeToUpdate.UserId != userId)
                return RedirectToPage("./Index");

            

            if (await TryUpdateModelAsync<Recipe>(recipeToUpdate, "Recipe",
                r => r.Name, r => r.Description))
            {
                UpdateLists(recipeToUpdate);
                recipeToUpdate.ModificationDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToPage("./Details", new { id = recipeId });
            }
            return RedirectToPage("./Index");
        }

        public void UpdateLists(Recipe recipeToUpdate)
        {
            var categories = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);
            var ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngredientsSession);
            var images = _utilsService.GetSession<List<Image>>(HttpContext, ImagesSession);
            if (categories != null)
            {
                foreach (var categoryId in categories)
                {
                    if (recipeToUpdate.CategoryRecipes.FirstOrDefault(c => c.CategoryId == categoryId) == null)
                    {
                        recipeToUpdate.CategoryRecipes.Add(new CategoryRecipe { CategoryId = categoryId, RecipeId = recipeToUpdate.RecipeId });
                    }
                }
                foreach (var category in recipeToUpdate.CategoryRecipes)
                {
                    if (!categories.Contains(category.CategoryId))
                    {
                        CategoryRecipe categoryToRemove = recipeToUpdate.CategoryRecipes.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                        _context.CategoryRecipes.Remove(categoryToRemove);
                    }
                }
            }
            if (ingredients != null)
            {
                foreach (var ingredient in ingredients)
                {
                    if (recipeToUpdate.Ingredients.FirstOrDefault(i => i.Name == ingredient) == null)
                    {
                        recipeToUpdate.Ingredients.Add(new Ingredient { Name = ingredient });
                    }
                }
                foreach (var ingredient in recipeToUpdate.Ingredients)
                {
                    if (!ingredients.Contains(ingredient.Name))
                    {
                        Ingredient ingredientToRemove = recipeToUpdate.Ingredients.FirstOrDefault(i => i.Name == ingredient.Name);
                        _context.Ingredients.Remove(ingredientToRemove);
                    }
                }
            }
            if (images != null)
            {
                foreach (var image in recipeToUpdate.Images)
                {
                    if (!images.Exists(i => i.Name == image.Name))
                    {
                        Image imageToRemove = recipeToUpdate.Images.FirstOrDefault(i => i.Name == image.Name);
                        _imagesService.DeleteImage(imageToRemove.Name);
                        _context.Images.Remove(imageToRemove);
                    }
                }
                foreach(Image image in images)
                {
                    if(!recipeToUpdate.Images.Any(i => i.URL == image.URL))
                    {
                        Image NewImage = image;
                        NewImage.RecipeId = recipeToUpdate.RecipeId;
                        _context.Images.Add(image);
                    }
                }
            }
        }

        public void OnPostAddIngredient()
        {
            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngredientsSession);
            if (Ingredients == null)
                Ingredients = new List<string>();

            if (!string.IsNullOrWhiteSpace(NewIngredient) && !Ingredients.Contains(NewIngredient))
                Ingredients.Add(NewIngredient);

            _utilsService.SetSession(HttpContext, IngredientsSession, Ingredients);

            RefreshPageModels();
        }

        public void OnPostDeleteIngredient(String name)
        {
            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngredientsSession);

            if (Ingredients == null)
                Ingredients = new List<string>();

            if (Ingredients.Contains(name))
                Ingredients.Remove(name);

            _utilsService.SetSession(HttpContext, IngredientsSession, Ingredients);

            RefreshPageModels();
        }

        public void OnPostAddCategory()
        {
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (!CategoriesId.Contains((int)NewCategory))
                CategoriesId.Add((int)NewCategory);

            _utilsService.SetSession(HttpContext, CategoriesSession, CategoriesId);

            RefreshPageModels();
        }

        public void OnPostDeleteCategory(int id)
        {
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);

            if (CategoriesId == null)
                CategoriesId = new List<int>();

            if (CategoriesId.Contains(id))
                CategoriesId.Remove(id);

            _utilsService.SetSession(HttpContext, CategoriesSession, CategoriesId);

            RefreshPageModels();
        }

        public async Task OnPostAddImage()
        {
            Images = _utilsService.GetSession<List<Image>>(HttpContext, ImagesSession);

            if (Images == null)
                Images = new List<Image>();

            if (NewImage != null)
            {
                Images.Add(new Image { Name = await _imagesService.UploadImage(NewImage, Recipe.Name) });
            }

            _utilsService.SetSession(HttpContext, ImagesSession, Images);

            RefreshPageModels();
        }

        public void OnPostDeleteImage(int index)
        {
            Images = _utilsService.GetSession<List<Image>>(HttpContext, ImagesSession);

            if (Images == null)
                Images = new List<Image>();

            if (index < Images.Count)
            {
                if( _context.Images.Where(i => i.RecipeId == _utilsService.GetSession<int>(HttpContext, RecipeIdSession)).Any(i => i.Name == Images[index].Name))
                {
                    if (System.IO.File.Exists(Images[index].GetUrl()))
                    {
                        System.IO.File.Delete(Images[index].GetUrl());
                    }
                }
                Images.RemoveAt(index);
            }

            _utilsService.SetSession(HttpContext, ImagesSession, Images);

            RefreshPageModels();
        }

        public void RefreshPageModels()
        {
            CategoriesId = _utilsService.GetSession<List<int>>(HttpContext, CategoriesSession);
            Ingredients = _utilsService.GetSession<List<string>>(HttpContext, IngredientsSession);
            Images = _utilsService.GetSession<List<Image>>(HttpContext, ImagesSession);
        }
    }
}
