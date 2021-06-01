using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Services
{
    public class FavouritiesService
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseRecipesService _recipesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavouritiesService(ApplicationDbContext context, DatabaseRecipesService recipesService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _recipesService = recipesService;
            _userManager = userManager;
        }

        public async Task AddRemoveFav(Recipe recipeAdded, ApplicationUser userAdding)
        {
            if (recipeAdded == null)
            {
                throw new NullReferenceException();
            }

            if (userAdding == null)
            {
                throw new NullReferenceException();
            }

            var favourite = await FindFavouriteAsync(recipeAdded.RecipeId, userAdding.Id);
           
            if (favourite == null)
            {
                await AddFav(recipeAdded, userAdding);
            }
            else
            {
                await RemoveFav(userAdding, favourite);
            }
        }

        public async Task RemoveFav(ApplicationUser userAdding, Favourite favourite)
        {
            userAdding.Favourites.Remove(favourite);
            await _userManager.UpdateAsync(userAdding);
        }

        public async Task AddFav(Recipe recipeAdded, ApplicationUser userAdding)
        {
            var newFavourite = new Favourite()
            {
                Recipe = recipeAdded,
                User = userAdding
            };
            userAdding.Favourites.Add(newFavourite);
            await _userManager.UpdateAsync(userAdding);
        }

        public async Task<Favourite> FindFavouriteAsync(int recipeId, string userId)
        {
            var favourite = await _context.Favourites.FirstOrDefaultAsync(v => v.UserId == userId && v.RecipeId == recipeId);
            return favourite;
        }

        public async Task<IEnumerable<Favourite>> FindFavouritesByUserIdAsync(ApplicationUser userAdding)
        {   
            var favourite = await _context.Favourites
                .Include(r => r.Recipe)
                .ThenInclude(v => v.Votes)
                .Where(v => v.UserId == userAdding.Id)
                .ToListAsync();
            return favourite;
        }
    }
}
