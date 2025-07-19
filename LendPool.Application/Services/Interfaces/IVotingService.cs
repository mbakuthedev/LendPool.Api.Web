using LendPool.Domain.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IVotingService
    {
        Task<GenericResponse<string>> CastVoteAsync(string lenderId, CastVoteDto dto);
        Task<GenericResponse<VoteResultDto>> GetVoteResultAsync(string loanRequestId);
        Task<GenericResponse<List<VoteResultDto>>> GetVoteResultsByPoolAsync(string poolId);
        Task<GenericResponse<PoolVotingSummaryDto>> GetPoolVotingSummaryAsync(string poolId);
        Task<GenericResponse<bool>> HasVotedAsync(string lenderId, string loanRequestId);
        Task<GenericResponse<int>> GetPoolMemberCountAsync(string poolId);
        Task<GenericResponse<int>> GetActiveVoterCountAsync(string loanRequestId);
    }
} 