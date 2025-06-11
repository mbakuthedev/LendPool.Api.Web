using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Application.Services.Implementation
{
    public class InterestPaymentService : IInterestPaymentService
    {
        private readonly ApplicationDbContext _context;

        public InterestPaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<bool>> RecordInterestPaymentAsync(string poolId, string loanId, string userId, decimal amount)
        {
            var interestPayment = new InterestPayment
            {
                PoolId = poolId,
                LoanId = loanId,
                Amount = amount,
                PaidByUserId = userId,
                PaymentDate = DateTime.UtcNow
            };

            _context.InterestPayments.Add(interestPayment);

            var success = await _context.SaveChangesAsync() > 0;
            return GenericResponse<bool>.SuccessResponse(success, 200, "Interest payment recorded");
        }

        public async Task<decimal> GetTotalInterestEarnedAsync(string poolId)
        {
            return await _context.InterestPayments
                .Where(x => x.PoolId == poolId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }

        public async Task<List<InterestPayment>> GetInterestPaymentsByPoolAsync(string poolId)
        {
            return await _context.InterestPayments
                .Where(x => x.PoolId == poolId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();
        }
    }

}
