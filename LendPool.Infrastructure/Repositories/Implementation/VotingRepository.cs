using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class VotingRepository : IVotingRepository
    {
        private readonly ApplicationDbContext _context;

        public VotingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<Vote>> AddVoteAsync(Vote vote)
        {
            try
            {
                await _context.Votes.AddAsync(vote);
                await _context.SaveChangesAsync();
                return GenericResponse<Vote>.SuccessResponse(vote, 201, "Vote recorded successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<Vote>.FailResponse($"Error adding vote: {ex.Message}");
            }
        }

        public async Task<GenericResponse<List<Vote>>> GetVotesByLoanRequestAsync(string loanRequestId)
        {
            try
            {
                var votes = await _context.Votes
                    .Include(v => v.Lender)
                    .Where(v => v.LoanRequestId == loanRequestId)
                    .ToListAsync();

                return GenericResponse<List<Vote>>.SuccessResponse(votes, 200, "Votes retrieved successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Vote>>.FailResponse($"Error retrieving votes: {ex.Message}");
            }
        }

        public async Task<GenericResponse<bool>> HasVotedAsync(string lenderId, string loanRequestId)
        {
            try
            {
                var hasVoted = await _context.Votes
                    .AnyAsync(v => v.LenderId == lenderId && v.LoanRequestId == loanRequestId);

                return GenericResponse<bool>.SuccessResponse(hasVoted, 200, "Vote status checked");
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.FailResponse($"Error checking vote status: {ex.Message}");
            }
        }

        public async Task<GenericResponse<int>> GetPoolMemberCountAsync(string poolId)
        {
            try
            {
                var memberCount = await _context.LenderPoolMemberships
                    .CountAsync(m => m.LenderPoolId == poolId);

                return GenericResponse<int>.SuccessResponse(memberCount, 200, "Pool member count retrieved");
            }
            catch (Exception ex)
            {
                return GenericResponse<int>.FailResponse($"Error getting pool member count: {ex.Message}");
            }
        }

        public async Task<GenericResponse<int>> GetActiveVoterCountAsync(string loanRequestId)
        {
            try
            {
                var activeVoterCount = await _context.Votes
                    .Where(v => v.LoanRequestId == loanRequestId)
                    .CountAsync();

                return GenericResponse<int>.SuccessResponse(activeVoterCount, 200, "Active voter count retrieved");
            }
            catch (Exception ex)
            {
                return GenericResponse<int>.FailResponse($"Error getting active voter count: {ex.Message}");
            }
        }

        public async Task<GenericResponse<List<Vote>>> GetVotesByPoolAsync(string poolId)
        {
            try
            {
                var votes = await _context.Votes
                    .Include(v => v.Lender)
                    .Include(v => v.LoanRequest)
                    .Where(v => v.LoanRequest.MatchedPoolId == poolId)
                    .ToListAsync();

                return GenericResponse<List<Vote>>.SuccessResponse(votes, 200, "Pool votes retrieved successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<List<Vote>>.FailResponse($"Error retrieving pool votes: {ex.Message}");
            }
        }

        public async Task<GenericResponse<Vote>> GetVoteByLenderAndRequestAsync(string lenderId, string loanRequestId)
        {
            try
            {
                var vote = await _context.Votes
                    .Include(v => v.Lender)
                    .FirstOrDefaultAsync(v => v.LenderId == lenderId && v.LoanRequestId == loanRequestId);

                if (vote == null)
                    return GenericResponse<Vote>.FailResponse("Vote not found", 404);

                return GenericResponse<Vote>.SuccessResponse(vote, 200, "Vote retrieved successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<Vote>.FailResponse($"Error retrieving vote: {ex.Message}");
            }
        }

        public async Task<GenericResponse<bool>> UpdateVoteAsync(Vote vote)
        {
            try
            {
                _context.Votes.Update(vote);
                await _context.SaveChangesAsync();
                return GenericResponse<bool>.SuccessResponse(true, 200, "Vote updated successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.FailResponse($"Error updating vote: {ex.Message}");
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 