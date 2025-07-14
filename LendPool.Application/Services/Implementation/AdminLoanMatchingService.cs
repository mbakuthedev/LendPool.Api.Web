using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Application.Services.Implementation
{
    public class AdminLoanMatchingService : IAdminLoanMatchingService
    {
        public Task<List<UnmatchedLoanRequestDto>> GetUnmatchedLoanRequestsAsync() =>
            Task.FromResult(new List<UnmatchedLoanRequestDto>
            {
                new UnmatchedLoanRequestDto { LoanRequestId = "1", BorrowerName = "Jane Doe", Amount = 500000, Purpose = "Business Expansion", DurationMonths = 12 },
                new UnmatchedLoanRequestDto { LoanRequestId = "2", BorrowerName = "John Smith", Amount = 200000, Purpose = "Medical Bills", DurationMonths = 6 }
            });

        public Task<List<LenderPoolDto>> GetLenderPoolsAsync() =>
            Task.FromResult(new List<LenderPoolDto>
            {
                new LenderPoolDto { PoolId = "pool1", PoolName = "Alpha Pool" },
                new LenderPoolDto { PoolId = "pool2", PoolName = "Beta Pool" }
            });

        public Task<bool> AssignPoolToLoanRequestAsync(string loanRequestId, string poolId)
            => Task.FromResult(true); // TODO: Implement real logic
    }
} 