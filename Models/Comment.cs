using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalKulinarny.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalKulinarny.Models
{
    public class Comment
    {
        public int id { get; set; }
        
        [DisplayName("Komentarz")]
        [Required(ErrorMessage = "Zawartość jest wymagana!")]
        public string comment { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser user { get; set; }
        
        public int RecipeId { get; set; }
        public Recipe recipe { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime modificationDate { get; set; }

        public ICollection<CommentLike> commentsLikes { get; set; }
    }
}
