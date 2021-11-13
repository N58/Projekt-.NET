using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalKulinarny.Areas.Identity.Data;

namespace PortalKulinarny.Models
{
    public class CommentLike
    {
        public string? UserId { get; set; }
        public ApplicationUser? user { get; set; }
        public int CommentId { get; set; }
        public Comment comment { get; set; }
    }
}
