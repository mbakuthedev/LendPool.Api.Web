using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class RepaymentRepository : IRepaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public RepaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Repayment>> GetRepaymentsByPoolIdAsync(string poolId)
        {
            return await _context.Repayments
                .Include(r => r.Loan)
                .Where(r => r.LenderpoolId == poolId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRepaidForLoanAsync(string loanId)
        {
            return await _context.Repayments
                .Where(r => r.LoanId == loanId)
                .SumAsync(r => r.AmountPaid);
        }

        public async Task<Loan> GetLoanByIdAsync(string loanId)
        {
            return await _context.Loans
                .FirstOrDefaultAsync(l => l.Id == loanId);
        }

        public async Task<GenericResponse<Repayment>> AddRepayment(Repayment repayment)
        {
           await  _context.Repayments.AddAsync(repayment);

           await  _context.SaveChangesAsync();

            return GenericResponse<Repayment>.SuccessResponse(repayment, 201, "Success adding repayment ");
        }

        
    }

}
