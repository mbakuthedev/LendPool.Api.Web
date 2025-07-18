using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Data;
using LendPool.Domain.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class LenderDashboardService : ILenderDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LenderDashboardService> _logger;
        public LenderDashboardService(ApplicationDbContext context, ILogger<LenderDashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

   
        public async Task<GenericResponse<List<FundedLoanDto>>> GetFundedLoansAsync(string lenderId)
        {
            _logger.LogInformation("Fetching funded loans for lender {LenderId}", lenderId);
            // 1. Get all pool IDs where the lender is a member
            var poolIds = await _context.LenderPoolMemberships
                .Where(m => m.UserId == lenderId)
                .Select(m => m.LenderPoolId)
                .ToListAsync();

            // 2. Get all loans in those pools
            var loans = await _context.Loans
                .Where(l => poolIds.Contains(l.PoolId))
                .Include(l => l.User)
                .Include(l => l.Pool)
                .ToListAsync();

            // 3. Map to FundedLoanDto
            var fundedLoans = loans.Select(l => new FundedLoanDto
            {
                Borrower = l.User?.FullName ?? "Unknown",
                Amount = l.Amount,
                Pool = l.Pool?.Name ?? "Unknown",
                Status = l.LoanStatus,
                Date = l.StartDate.ToString("yyyy-MM-dd"),
                
            }).ToList();

            _logger.LogInformation("Found {Count} funded loans for lender {LenderId}", fundedLoans.Count, lenderId);
            return GenericResponse<List<FundedLoanDto>>.SuccessResponse(fundedLoans, 200, "Sucess getting the funded loans");
        }

        public async Task<GenericResponse<LenderEarningsResponse>> GetRepaymentsAndEarningsAsync(string lenderId)
        {
            _logger.LogInformation("Fetching repayments and earnings for lender {LenderId}", lenderId);
            // 1. Get all pool IDs where the lender is a member
            var poolIds = await _context.LenderPoolMemberships
                .Where(m => m.UserId == lenderId)
                .Select(m => m.LenderPoolId)
                .ToListAsync();

            if (poolIds == null || !poolIds.Any())
            {
                _logger.LogWarning("Lender {LenderId} does not belong to any pools", lenderId);
                return GenericResponse<LenderEarningsResponse>.FailResponse("You don't belong to any pools", 404);
            }

            // 2. Get all loans in those pools
            var loanIds = await _context.Loans
                .Where(l => poolIds.Contains(l.PoolId))
                .Select(l => l.Id)
                .ToListAsync();

            if (loanIds == null || !loanIds.Any())
            {
                _logger.LogWarning("Lender {LenderId} does not have any loans", lenderId);
                return GenericResponse<LenderEarningsResponse>.FailResponse("You don't have to any loans ", 404);
            }
            // 3. Get all repayments for those loans
            var repayments = await _context.Repayments
                .Where(r => loanIds.Contains(r.LoanId))
                .ToListAsync();

            // 4. Map to LenderRepaymentDto
            var repaymentDtos = repayments.Select(r => new LenderRepaymentDto
            {
                Loan = r.LoanId,
                Amount = r.AmountPaid,
                Date = r.PaymentDate.ToString("yyyy-MM-dd"),
                Status = r.IsFullyRepaid ? "paid" : (r.IsLate ? "late" : "pending")
            }).ToList();

            // 5. Calculate total earnings (sum of AmountPaid)
            var totalEarnings = repayments.Sum(r => r.AmountPaid);

            _logger.LogInformation("Lender {LenderId} has {Count} repayments, total earnings: {TotalEarnings}", lenderId, repaymentDtos.Count, totalEarnings);
            return GenericResponse<LenderEarningsResponse>.SuccessResponse(new LenderEarningsResponse
            {
                TotalEarnings = totalEarnings,
                Repayments = repaymentDtos
            }, 200, "Success");
        }

        public async Task<GenericResponse<List<PoolPerformanceDto>>> GetPoolPerformanceAsync(string lenderId)
        {
            _logger.LogInformation("Fetching pool performance for lender {LenderId}", lenderId);

            // 1. Get all pools the lender is a member of
            var pools = await _context.LenderPoolMemberships
                .Where(m => m.UserId == lenderId)
                .Select(m => m.LenderPool)
                .Distinct()
                .ToListAsync();

            if (pools == null || !pools.Any())
            {
                _logger.LogWarning("Lender {LenderId} does not belong to any pools", lenderId);
                return GenericResponse<List<PoolPerformanceDto>>.FailResponse("you dont belong to any pools", 404);
            }

            var result = new List<PoolPerformanceDto>();

            foreach (var pool in pools)
            {
                // Get total repaid for this pool
                var totalRepaid = await _context.Repayments
                    .Where(r => r.LenderpoolId == pool.Id)
                    .SumAsync(r => (decimal?)r.AmountPaid) ?? 0;

                // Use pool.TotalCapital for denominator
                var totalCapital = pool.TotalCapital;
                decimal performance = (totalCapital > 0) ? totalRepaid / totalCapital : 0;
                result.Add(new PoolPerformanceDto
                {
                    Pool = pool.Name,
                    PerformanceValue = performance
                });
            }
            _logger.LogInformation("Lender {LenderId} pool performance calculated for {Count} pools", lenderId, result.Count);
            return GenericResponse<List<PoolPerformanceDto>>.SuccessResponse(result, 200, "Sucess getting pool performance");
        }

     
    }
} 