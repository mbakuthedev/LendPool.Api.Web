using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IVotingRepository
    {
        Task<GenericResponse<Vote>> AddVoteAsync(Vote vote);
        Task<GenericResponse<List<Vote>>> GetVotesByOperationAsync(string operationId, VoteOperationType operationType);
        Task<GenericResponse<bool>> HasVotedAsync(string lenderId, string operationId, VoteOperationType operationType);
        Task<GenericResponse<int>> GetPoolMemberCountAsync(string poolId);
        Task<GenericResponse<int>> GetActiveVoterCountAsync(string operationId, VoteOperationType operationType);
        Task<GenericResponse<List<Vote>>> GetVotesByPoolAsync(string poolId);
        Task<GenericResponse<Vote>> GetVoteByLenderAndOperationAsync(string lenderId, string operationId, VoteOperationType operationType);
        Task<GenericResponse<bool>> UpdateVoteAsync(Vote vote);
        Task SaveChangesAsync();
    }
} 