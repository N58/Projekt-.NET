using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Pages.Recipes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public readonly UserService _utilsService;

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public IndexModel(ApplicationDbContext context, UserService utilsService)
        {
            _context = context;
            _utilsService = utilsService;
        }

        public IList<Recipe> Recipes { get; set; }

        public async Task OnGetAsync()
        {
            var recipes = from n in _context.Recipes
                          select n;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                recipes = recipes.Where(s => s.Name.Contains(Search));
            }
            
            Recipes = await recipes.ToListAsync();
        }
    }
}
