using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalKulinarny.Areas.Identity.Data;

namespace PortalKulinarny.Models
{
    public class Favourite
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
