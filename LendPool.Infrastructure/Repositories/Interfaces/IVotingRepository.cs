using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IVotingRepository
    {
        Task<GenericResponse<Vote>> AddVoteAsync(Vote vote);
        Task<GenericResponse<List<Vote>>> GetVotesByLoanRequestAsync(string loanRequestId);
        Task<GenericResponse<bool>> HasVotedAsync(string lenderId, string loanRequestId);
        Task<GenericResponse<int>> GetPoolMemberCountAsync(string poolId);
        Task<GenericResponse<int>> GetActiveVoterCountAsync(string loanRequestId);
        Task<GenericResponse<List<Vote>>> GetVotesByPoolAsync(string poolId);
        Task<GenericResponse<Vote>> GetVoteByLenderAndRequestAsync(string lenderId, string loanRequestId);
        Task<GenericResponse<bool>> UpdateVoteAsync(Vote vote);
        Task SaveChangesAsync();
    }
} 