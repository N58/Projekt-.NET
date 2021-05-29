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

namespace PortalKulinarny.Services
{
    public class VoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseRecipesService _recipesService;

        public VoteService(ApplicationDbContext context, DatabaseRecipesService recipesService)
        {
            _context = context;
            _recipesService = recipesService;
        }

        public async Task UpVote(Recipe recipeVoted, ApplicationUser userVoting)
        {
            const int upVoteValue = 1;
            await Vote(recipeVoted, userVoting, upVoteValue);
        }

        public async Task DownVote(Recipe recipeVoted, ApplicationUser userVoting)
        {
            const int downVoteValue = -1;
            await Vote(recipeVoted, userVoting, downVoteValue);
        }


        public async Task Vote(Recipe recipeVoted, ApplicationUser userVoting, int voteValue)
        {
            if (recipeVoted == null)
            {
                throw new NullReferenceException();
            }

            if (userVoting == null)
            {
                throw new NullReferenceException();
            }

            if( recipeVoted.UserId == userVoting.Id)
            {
                throw new InvalidOperationException();
            }

            var vote = await FindVoteAsync(recipeVoted, userVoting);
            if (vote == null)
            {
                await CreateVote(recipeVoted, userVoting, voteValue);
            }
            else
            {
                await RemoveVote(recipeVoted, vote);
                await CreateVote(recipeVoted, userVoting, voteValue);
            }
        }

        public async Task RemoveVote(Recipe recipeVoted, Vote vote)
        {
            recipeVoted.Rating -= vote.Value;
            recipeVoted.Votes.Remove(vote);
            await _recipesService.Update(recipeVoted);
        }

        public async Task CreateVote(Recipe recipeVoted, ApplicationUser userVoting, int value)
        {
            var newVote = new Vote()
            {
                Recipe = recipeVoted,
                User = userVoting,
                Value = value
            };
            recipeVoted.Votes.Add(newVote);
            recipeVoted.Rating += newVote.Value;
            await _recipesService.Update(recipeVoted);
        }

        public async Task<Vote> FindVoteAsync(Recipe recipeVoted, ApplicationUser userVoting)
        {
            var vote =  await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userVoting.Id && v.RecipeId == recipeVoted.RecipeId);
            return vote;
        }
    }
}
