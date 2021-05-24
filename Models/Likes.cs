using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PortalKulinarny.Models
{
    public class Likes
    {
        [Key]
        public string UserId { get; set; }

        [Key]
        public int RecipeID { get; set; }
        public Recipe Recipe { get; set; }
    }
}
