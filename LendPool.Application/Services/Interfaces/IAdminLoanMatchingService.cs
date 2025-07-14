using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;

namespace LendPool.Application.Services.Interfaces
{
    public interface IAdminLoanMatchingService
    {
        Task<List<UnmatchedLoanRequestDto>> GetUnmatchedLoanRequestsAsync();
        Task<List<LenderPoolDto>> GetLenderPoolsAsync();
        Task<bool> AssignPoolToLoanRequestAsync(string loanRequestId, string poolId);
    }
} 