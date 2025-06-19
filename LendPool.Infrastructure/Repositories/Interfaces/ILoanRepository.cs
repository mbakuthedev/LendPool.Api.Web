using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<GenericResponse<LoanRequest>> AddLoanRequestAsync(LoanRequest request);
        Task<GenericResponse<LoanRequest>> GetLoanRequestByIdAsync(string requestId);
        Task<GenericResponse<List<LoanRequest>>> GetLoanRequestsByUserAsync(string userId);
        Task<GenericResponse<List<LoanRequest>>> GetAllLoanRequestsAsync();

        Task<GenericResponse<Loan>> GetLoanByIdAsync(string id);
        Task<GenericResponse<List<Loan>>> GetLoansByUserAsync(string userId);
        Task<GenericResponse<List<Loan>>> GetAllLoansAsync();
        Task<GenericResponse<List<Loan>>> GetLoansByPoolAsync(string poolId);
        Task<GenericResponse<Loan>> AddLoanAsync(Loan loan);

        Task<GenericResponse<Wallet>> GetOrCreateWalletAsync(string userId);
        Task<GenericResponse<Transaction>> AddTransactionAsync(Transaction transaction);

        Task<GenericResponse<Repayment>> AddRepaymentAsync(Repayment repayment);
        Task<GenericResponse<List<Repayment>>> GetRepaymentsByUserAsync(string userId);
        Task<GenericResponse<List<Repayment>>> GetRepaymentsByLoanAsync(string loanId);

        Task SaveChangesAsync();

    }
}
