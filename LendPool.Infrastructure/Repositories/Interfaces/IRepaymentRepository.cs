using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IRepaymentRepository
    {
        Task<IEnumerable<Repayment>> GetRepaymentsByPoolIdAsync(string poolId);
        Task<decimal> GetTotalRepaidForLoanAsync(string loanId);
        Task<Loan> GetLoanByIdAsync(string loanId);
        Task<GenericResponse<Repayment>> AddRepayment(Repayment repayment);
    }

}
