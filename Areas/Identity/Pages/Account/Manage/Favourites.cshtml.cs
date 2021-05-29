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
using PortalKulinarny.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PortalKulinarny.Areas.Identity.Pages.Account.Manage
{
    public class Favourities : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public List<Recipe> Favourites { get; set; }
        public Favourities(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            Favourites = new List<Recipe>();
        }
        public async Task OnGetAsync()
        {   
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.Include(u => u.Favourites).ThenInclude(fr => fr.Recipe).FirstOrDefaultAsync(u => u.Id == userID);
            foreach(var i in user.Favourites)
            {
                Favourites.Add(i.Recipe);
            }
        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
