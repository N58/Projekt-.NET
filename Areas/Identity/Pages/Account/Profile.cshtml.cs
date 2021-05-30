using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Areas.Identity.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseRecipesService _recipesService;
        private readonly FavouritiesService _favouritiesService;

        public ProfileModel(
            UserManager<ApplicationUser> userManager, DatabaseRecipesService recipesService, FavouritiesService favouritiesService)
        {
            _userManager = userManager;
            _recipesService = recipesService;
            _favouritiesService = favouritiesService;
        }

        public ApplicationUser User { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            User = user;
            User.Recipes = (ICollection<Recipe>)await _recipesService.FindByUserIdAsync(user);
            User.Favourites = (ICollection<Favourite>)await _favouritiesService.FindFavouritesByUserIdAsync(user);
        }
    }
}
