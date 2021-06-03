using PortalKulinarny.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Musisz podać nazwę kategorii!")]
        public string Name { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<CategoryRecipe> CategoryRecipes { get; set; }
    }
}
