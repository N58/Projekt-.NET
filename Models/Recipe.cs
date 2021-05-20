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

        public string UserId { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwę przepisu!")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Musisz podać opis wykonania!")]
        [Display(Name = "Opis wykonania")]
        public string Description { get; set; }

        [Display(Name = "Data")]
        public DateTime DateTime { get; set; }

        
    }
}
