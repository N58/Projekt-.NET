using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class CategoryRecipe
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
