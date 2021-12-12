using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseRecipesService _recipesService;
        public readonly VoteService _voteService;
        public readonly FavouritiesService _favouritiesService;
        public readonly UserService _userService;
        private readonly ImagesService _imagesService;
        public int? editCommentId;
        [BindProperty]
        public Comment NewComment { get; set; }
        [BindProperty]
        public Comment EditComment { get; set; }
        [BindProperty]
        public Sort sortComments { get; set; }

        public class Sort
        {
            public string sort { get; set; }
            public Sort()
            {
                sort = "createAt";
            }
        }

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            DatabaseRecipesService recipesService, VoteService voteService, FavouritiesService favouritiesService, UserService utilsService,
            ImagesService imagesService)
        {
            _context = context;
            _userManager = userManager;
            _recipesService = recipesService;
            _voteService = voteService;
            _favouritiesService = favouritiesService;
            _userService = utilsService;
            _imagesService = imagesService;

            sortComments = new Sort();
        }
        [BindProperty]
        public Recipe Recipe { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        public List<Comment> comments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.sortComments = sortComments;
            if (id == null)
            {
                return NotFound();
            }

            await LoadAsync(id);

            if (Recipe == null)
            {
                return NotFound();
            }
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Recipe.UserId != UserId)
                Recipe.ViewCount++;
            await _context.SaveChangesAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostSortAsync(int? id)
        {
            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostUpVoteAsync(int? id)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.UpVote(recipeVoted, userVoting);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(id);
            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {

            if (NewComment.comment == null || NewComment.comment =="")
                {
                await LoadAsync(id);
                return Page();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            NewComment.RecipeId = (int)id;
            NewComment.UserId = userId;
            NewComment.createdAt = DateTime.Now;
            NewComment.modificationDate = DateTime.Now;
            await _context.Comments.AddAsync(NewComment);
            _context.SaveChanges();
            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostDownVoteAsync(int? id)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.DownVote(recipeVoted, userVoting);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }
            await LoadAsync(id);
            return Page();

        }

        public async Task<IActionResult> OnPostLikeAsync(int id, int commentId)
        {
            try
            {
                _context.CommentsLikes.Add(new CommentLike()
                {
                    CommentId = commentId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                });
                _context.SaveChanges();
            }
            catch (Exception e)
            {

            }
            await LoadAsync(id);
            return Page();
        }


        public async Task<IActionResult> OnPostUnlikeAsync(int id, int commentId)
        {
            _context.CommentsLikes.Remove(new CommentLike()
            {
                CommentId = commentId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
            _context.SaveChanges();
            await LoadAsync(id);
            return Page();

        }

        public async Task<IActionResult> OnPostSaveEditAsync(int id,int commentId)
        {
            if (EditComment.comment == null || EditComment.comment == "")
            {
                await LoadAsync(id);
                return Page();
            }
            NewComment = _context.Comments.FirstOrDefault(x=>x.id == commentId);
            NewComment.modificationDate = DateTime.Now;
            NewComment.comment = EditComment.comment;   
            _context.Update(NewComment);
                _context.SaveChanges();
            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(int id, int commentId)
        {
           
            try
            {

                EditComment = _context.Comments.FirstOrDefault(x => x.id == commentId);
                editCommentId = EditComment.id;
            }
            catch (Exception e) { }
            await LoadAsync(id);
            return Page();
        }
        public async Task<IActionResult> OnPostCommentDeleteAsync(int id, int commentId)
        {
            try
            {
                var c = new Comment();
                c.id = commentId;
                _context.Comments.Remove(c);
                _context.SaveChanges();
            }
            catch (Exception) { }
            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostFavouritiesAsync(int id)
        {
            var recipeAdded = await _recipesService.FindByIdAsync(id);

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userAdding = await _context.Users.Include(u => u.Favourites).FirstOrDefaultAsync(u => u.Id == userID);

            try
            {
                await _favouritiesService.AddRemoveFav(recipeAdded, userAdding);
            }
            catch
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }
            await LoadAsync(id);
            return Page();


        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.Include(r => r.Images).FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (recipe.UserId == null || recipe.UserId != userId)
                return RedirectToPage("./Index");

            foreach (var image in recipe.Images)
                _imagesService.DeleteImage(image.Name);

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            await LoadAsync(id);    
            return RedirectToPage("./Index");
        }

        public async Task LoadAsync(int? id)
        {
            Recipe = await _recipesService.FindByIdAsync(id);
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var c = _context.Comments
                .Where(e => e.RecipeId == id)
                .Include(e => e.recipe)
                .Include(e => e.user)
                .Include(e => e.commentsLikes);

            switch (sortComments.sort)
            {
                case "createAt":
                    comments= c.OrderByDescending(x => x.createdAt).ToList();
                    break;

                case "modification":
                    comments = c.OrderByDescending(x => x.modificationDate).ToList();
                    break;
            }

            
            
        }
    }
}
