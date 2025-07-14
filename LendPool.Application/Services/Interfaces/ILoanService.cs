using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILoanService
    {
        Task<GenericResponse<Loan>> AddLoanAsync(CreateLoanDto loanDto);
        Task<GenericResponse<LoanRequestResponseDto>> SubmitLoanRequestAsync(string userId, LoanRequestDto dto);
        Task<GenericResponse<string>> ApproveLoanAsync(string lenderId, ApproveLoanRequestDto dto);
        Task<GenericResponse<List<LoanRequest>>> GetAllLoanRequestsAsync();
        Task<GenericResponse<string>> RejectLoanAsync(string lenderId, RejectLoanRequestDto dto);
        Task<GenericResponse<Loan>> GetLoanByIdAsync(string id);
        Task<GenericResponse<LoanRequest>> GetLoanRequestByIdAsync(string requestId);
        Task<GenericResponse<List<LoanRequest>>> GetLoanRequestsByUserAsync(string userId);
         Task<GenericResponse<List<Loan>>> GetLoansByPoolAsync(string poolId);
        Task<GenericResponse<List<Loan>>> GetLoansByUserAsync(string userId);
        Task<GenericResponse<Wallet>> GetOrCreateWalletAsync(string userId);
        Task<GenericResponse<List<Repayment>>> GetRepaymentsByLoanAsync(string loanId);
        Task<GenericResponse<List<Repayment>>> GetRepaymentsByUserAsync(string userId);
    }
}
