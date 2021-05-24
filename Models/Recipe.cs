using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Display(Name = "Użytkownik")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwę przepisu!")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Musisz podać opis wykonania!")]
        [Display(Name = "Opis wykonania")]
        public string Description { get; set; }

        [Display(Name = "Data")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Data modyfikacji")]
        public DateTime ModificationDateTime { get; set; }

        [Display(Name = "Składniki")]
        public ICollection<Ingredients> Ingredients { get; set; }
        [Display(Name = "Polubienia")]
        public ICollection<Likes> Likes { get; set; }
    }
}
