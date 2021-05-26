using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int RecipeID { get; set; }
        public Recipe Recipe { get; set; }
    }
}
