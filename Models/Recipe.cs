using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using PortalKulinarny.Areas.Identity.Data;

namespace PortalKulinarny.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        [Required(ErrorMessage = "Musisz podać nazwę przepisu!")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Musisz podać opis wykonania!")]
        [Display(Name = "Opis wykonania")]
        public string Description { get; set; }

        [Display(Name = "Data dodania")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Data ostatniej modyfikacji")]
        public DateTime ModificationDateTime { get; set; }
        [Display(Name = "Ocena")]
        [NotMapped]
        public int Rating { get => GetRating(); }
        [Display(Name = "Składniki")]
        public ICollection<Ingredient> Ingredients { get; set; }
        public ApplicationUser User { get; set;}
        [Display(Name = "Autor")]
        public string UserId { get; set; }

        public int GetRating()
        {
            int rating = 0;
            if(Votes != null)
                Votes.ToList().ForEach(v => rating += v.Value);
            return rating;
        }

        public ICollection<Favourite> Favourites { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public ICollection<CategoryRecipe> CategoryRecipes { get; set; }
    }
}
