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

        public string UserID { get; set; }
        [Display(Name = "U¿ytkownik")]
        public string Username { get; set; }
        [Display(Name = "Imiê")]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Przepisy")]
        public List<Recipe> Recipes { get; set; }
        [Display(Name = "Ulubione")]
        public List<Favourite> Favourites { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.FindByIdAsync(id)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            UserID = user.Id;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Recipes = (List<Recipe>)await _recipesService.FindByUserIdAsync(user);
            Favourites = (List<Favourite>)await _favouritiesService.FindFavouritesByUserIdAsync(user);
        }
    }
}
