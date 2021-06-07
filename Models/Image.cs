using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public string Name { get; set; }
        [NotMapped]
        [DisplayName("Dodaj zdjęcie")]
        public IFormFile ImageFile { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
