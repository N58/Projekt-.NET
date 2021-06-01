using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Favourites)
                .ThenInclude(f => f.Recipe)
                .ThenInclude(r => r.Votes)
                .Include(u => u.Recipes)
                .ThenInclude(r => r.Votes)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }
    }
}
