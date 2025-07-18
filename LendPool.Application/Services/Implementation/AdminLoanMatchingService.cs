using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Data;
using System;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Implementation
{
    public class AdminLoanMatchingService : IAdminLoanMatchingService
    {
        private readonly ApplicationDbContext _context;

        public AdminLoanMatchingService(ApplicationDbContext context)
        {
            _context = context;
        }
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

        public async Task<GenericResponse<bool>> AssignPoolToLoanRequestAsync(string loanRequestId, string poolId)
        {
            // Find the loan request
            var loanRequest = await _context.LoanRequests.FindAsync(loanRequestId);
            if (loanRequest == null)
                return GenericResponse<bool>.FailResponse("the loan request does not exist", 404);

            // Find the pool
            var pool = await _context.LenderPools.FindAsync(poolId);
            if (pool == null)
                return GenericResponse<bool>.FailResponse("pool does not exist", 404);

            // Assign the pool
            loanRequest.MatchedPoolId = poolId;
            loanRequest.MatchedPool = pool;
            loanRequest.DateModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return GenericResponse<bool>.SuccessResponse(true, 200, $"Loan request assigned to pool with id {poolId}");
        }
          
    }
} 