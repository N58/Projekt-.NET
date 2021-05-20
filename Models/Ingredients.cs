using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Ingredients
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("RecipeFK")]
        public int RecipeFK { get; set; }
    }
}
