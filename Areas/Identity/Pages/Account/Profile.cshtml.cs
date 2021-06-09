using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
        private readonly UserService _userService;

        public ProfileModel(UserService userService)
        {
            _userService = userService;
        }

        public ApplicationUser AppUser { get; set; }
        public bool IsOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }


            if (id == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                IsOwner = true;
            }
            else IsOwner = false;

            AppUser = user;

            AppUser.Recipes.OrderByDescending(r => r.DateTime);

            return Page();
        }
    }
}
