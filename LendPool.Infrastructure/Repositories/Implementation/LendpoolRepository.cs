using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class LendpoolRepository : ILendpoolRepository
    {
        private readonly ApplicationDbContext _context;

        public LendpoolRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserToPoolAsync(LenderPoolMembership lenderPoolMembership)
        {
            var membership = new LenderPoolMembership
            {
                Id = Guid.NewGuid().ToString(),
                UserId = lenderPoolMembership.UserId,
                LenderPoolId = lenderPoolMembership.LenderPoolId,
                JoinedAt = DateTime.UtcNow
            };

            await _context.LenderPoolMemberships.AddAsync(membership);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<GenericResponse<PoolSummaryDto>> GetPoolSummaryAsync(string poolId)
        {
            var pool = await _context.LenderPools
                .FirstOrDefaultAsync(p => p.Id == poolId);

            if (pool == null)
            {
                return GenericResponse<PoolSummaryDto>.FailResponse("Pool not found", 404);
            }

            // Total repaid by borrowers linked to this pool
            var totalRepaid = await _context.Repayments
                .Where(r => r.LenderpoolId == poolId)
                .SumAsync(r => (decimal?)r.AmountPaid) ?? 0;

            // Total earnings (e.g., interest payments)
            var totalEarnings = await _context.InterestPayments
                .Where(e => e.PoolId == poolId)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            // Count of active loans
            var activeLoansCount = await _context.Loans
                .CountAsync(l => l.PoolId == poolId && l.LoanStatus == LoanStatus.Active.ToString());

            var summary = new PoolSummaryDto
            {
                TotalCapital = pool.TotalCapital,
                TotalRepaid = totalRepaid,
                TotalEarnings = totalEarnings,
                ActiveLoansCount = activeLoansCount
            };

            return GenericResponse<PoolSummaryDto>.SuccessResponse(summary, 200, "Pool summary retrieved successfully.");
        }


        public async Task<GenericResponse<LenderPool>> CreateAsync(LenderPool pool)
        {
            _context.LenderPools.Add(pool);
            await _context.SaveChangesAsync();
            return GenericResponse<LenderPool>.SuccessResponse(pool, 200);
        }

        public async Task<GenericResponse<bool>> RecordWithdrawalAsync(string poolId, string userId, decimal amount)
        {
            var withdrawal = new PoolWithdrawal
            {
                Id = Guid.NewGuid().ToString(),
                PoolId = poolId,
                LenderId = userId,
                Amount = amount,
                DateCreated = DateTime.UtcNow
            };

            _context.PoolWithdrawals.Add(withdrawal);
            var saved = await _context.SaveChangesAsync() > 0;

            return saved
                ? GenericResponse<bool>.SuccessResponse(true, 200, "Withdrawal recorded successfully.")
                : GenericResponse<bool>.FailResponse("Failed to save withdrawal.", 500);
        }

        public async Task<bool> IsUserInPoolAsync(string userId, string poolId)
        {
            return await _context.LenderPoolMemberships
                .AnyAsync(m => m.UserId == userId && m.LenderPoolId == poolId);
        }


        public async Task<decimal> GetAvailableBalanceAsync(string poolId, string userId)
        {
            var totalContributed = await _context.PoolContributions
                .Where(pc => pc.PoolId == poolId && pc.LenderId == userId)
                .SumAsync(pc => pc.Amount);

            var totalWithdrawn = await _context.PoolWithdrawals
                .Where(w => w.PoolId == poolId && w.LenderId == userId)
                .SumAsync(w => w.Amount);

            return totalContributed - totalWithdrawn;
        }

        public async Task<GenericResponse<LenderPool>> GetByIdAsync(string id)
        {
            var pool =  await _context.LenderPools
                .Include(p => p.Contributions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pool == null)
            {
                return GenericResponse<LenderPool>.FailResponse("No pool found", 404);
            }

            return GenericResponse<LenderPool>.SuccessResponse(pool, 200);
        }

        public async Task<GenericResponse<IEnumerable<LenderPool>>> GetAllAsync()
        {
            var pools = await _context.LenderPools
               .Include(p => p.LenderPoolMemberships) 
               .ToListAsync();

            if (pools == null)
            {
                return GenericResponse<IEnumerable<LenderPool>>.FailResponse("No pool found", 404);
            }

            return GenericResponse<IEnumerable<LenderPool>>.SuccessResponse(pools, 200);
        }

        public async Task<GenericResponse<PaginatedResponse<LenderPool>>> GetAllPoolsWithMembersAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.LenderPools
                .Include(p => p.LenderPoolMemberships)
                    .ThenInclude(m => m.User)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var pools = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!pools.Any())
            {
                return GenericResponse<PaginatedResponse<LenderPool>>.FailResponse("No pools found", 404);
            }

            var result = new PaginatedResponse<LenderPool>
            {
                Data = pools,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };

            return GenericResponse<PaginatedResponse<LenderPool>>.SuccessResponse(result, 200);
        }


        public async Task<GenericResponse<IEnumerable<LoanDto>>> GetActiveLoansByPoolAsync(string poolId)
        {
            var loans =  await _context.Loans
                .Where(l => l.PoolId == poolId && l.IsActive)
                .ToListAsync();

            if (loans == null)
            {
                return GenericResponse<IEnumerable<LoanDto>>.FailResponse("No loan found", 404);
            }

            var loanDtos = loans.Select(loan => new LoanDto
            {
              
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                StartDate = loan.StartDate,
                DueDate = loan.DueDate,
                IsActive = loan.IsActive
            });

            return GenericResponse<IEnumerable<LoanDto>>.SuccessResponse(loanDtos, 200);
        }

        public async Task<GenericResponse<decimal>> GetTotalEarningsAsync(string poolId)
        {
            var earning =  await _context.Repayments
                .Where(r => r.LenderpoolId == poolId)
                .SumAsync(r => r.AmountPaid);

            if (earning < 0)
            {
                return GenericResponse<decimal>.FailResponse("No earning found", 404);
            }

            return GenericResponse<decimal>.SuccessResponse(earning, 200);
        }

        public async Task<GenericResponse<IEnumerable<Repayment>>> GetRepaymentsAsync(string poolId)
        {
            var repayment =  await _context.Repayments
                .Where(r => r.LenderpoolId == poolId)
                .ToListAsync();

            if (repayment == null)
            {
                return GenericResponse<IEnumerable<Repayment>>.FailResponse("No repayment found", 404);
            }

            return GenericResponse<IEnumerable<Repayment>>.SuccessResponse(repayment, 200);
        }

        public async Task<GenericResponse<bool>> ContributeAsync(PoolContribution contribution)
        {
            _context.PoolContributions.Add(contribution);
            var pool = await _context.LenderPools.FindAsync(contribution.PoolId);
            if (pool != null)
            {
                pool.TotalCapital += contribution.Amount;
            }
             var saved = await _context.SaveChangesAsync() > 0;

            return GenericResponse<bool>.SuccessResponse(saved, 200, "Sucessfully contributed ");
        }

        public async Task<GenericResponse<bool>> WithdrawAsync(string poolId, decimal amount)
        {
            var pool = await _context.LenderPools.FindAsync(poolId);
            if (pool == null || pool.TotalCapital < amount) return GenericResponse<bool>.FailResponse("No pool found or total capital is less than amount");

            pool.TotalCapital -= amount;
            var saved =  await _context.SaveChangesAsync() > 0;
            if (!saved)
            {
                return GenericResponse<bool>.FailResponse("an error occured when processing withdrawal");
            }

            return GenericResponse<bool>.SuccessResponse(saved, 200);
        }

        public async Task<GenericResponse<LenderPool>> GetPoolById(string poolId)
        {
            var pool = await _context.LenderPools.FindAsync(poolId);


            if (pool == null)
            {
                return GenericResponse<LenderPool>.FailResponse("No pools found", 404);
            }

            return GenericResponse<LenderPool>.SuccessResponse(pool, 200, "No pools found");
        }
    }
}
