using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using LendPool.Domain.Data;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _db;

        public LoanRepository(ApplicationDbContext loanRepository)
        {
            _db = loanRepository;
        }

        public async Task<GenericResponse<Loan>> AddLoanAsync(Loan loan)
        {
            await _db.Loans.AddAsync(loan);

            _db.SaveChanges();

            return GenericResponse<Loan>.SuccessResponse(loan, 201, "Sucess adding loans");
        }

        public async Task<GenericResponse<LoanRequest>> AddLoanRequestAsync(LoanRequest request)
        {
            try
            {
                _db.LoanRequests.Add(request);
                await _db.SaveChangesAsync();
                return GenericResponse<LoanRequest>.SuccessResponse(request, 201, "Loan request added");
            }
            catch (Exception ex)
            {
                return GenericResponse<LoanRequest>.FailResponse($"Error adding loan request: {ex.Message}");
            }
        }

        public async Task<GenericResponse<Repayment>> AddRepaymentAsync(Repayment repayment)
        {
            try
            {
                await _db.Repayments.AddAsync(repayment);

                await _db.SaveChangesAsync();

                return GenericResponse<Repayment>.SuccessResponse(repayment, 201, "Loan request added");
            }
            catch (Exception ex)
            {
                return GenericResponse<Repayment>.FailResponse($"Error adding loan request: {ex.Message}");
            }
        }

        public async Task<GenericResponse<Transaction>> AddTransactionAsync(Transaction transaction)
        {
            var newTransaction =  await _db.Transactions.AddAsync(transaction);
            if (newTransaction != null)
            {
                return GenericResponse<Transaction>.SuccessResponse(transaction, 200, "Sucess recording transaction");
            }
            return GenericResponse<Transaction>.FailResponse("Error recording transaction");
        }


        public async Task<GenericResponse<List<LoanRequest>>> GetAllLoanRequestsAsync()
        {
            var loanRequests = await _db.LoanRequests.ToListAsync();       

            return GenericResponse<List<LoanRequest>>.SuccessResponse(loanRequests, 200, "Sucessfully fetched loan requests");
        }

        public async Task<GenericResponse<List<Loan>>> GetAllLoansAsync()
        {
            var loans = await _db.Loans.ToListAsync();

            if (loans == null)
            {
                return GenericResponse<List<Loan>>.FailResponse("No loans found", 404);
            }

            return GenericResponse<List<Loan>>.SuccessResponse(loans, 200, "success fetching loans");
        }

        public async Task<GenericResponse<Loan>> GetLoanByIdAsync(string id)
        {
            var loan = await _db.Loans
                .Include(x => x.Pool)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (loan == null)
            {
                return GenericResponse<Loan>.FailResponse("No loans found", 404);
            }
            return GenericResponse<Loan>.SuccessResponse(loan, 200, "Sucessfully fetched loan");
        }

        public async Task<GenericResponse<LoanRequest>> GetLoanRequestByIdAsync(string requestId)
        {
            var result = await _db.LoanRequests.FindAsync(requestId);
            if (result == null)
                return GenericResponse<LoanRequest>.FailResponse("Loan request not found", 404);

            return GenericResponse<LoanRequest>.SuccessResponse(result, 200);
        }

        public async Task<GenericResponse<List<LoanRequest>>> GetLoanRequestsByUserAsync(string userId)
        {
             var loans = await _db.LoanRequests.Where(x => x.BorrowerId == userId).ToListAsync();

            if (loans == null)
            {
                return GenericResponse<List<LoanRequest>>.FailResponse("No loans found in this pool", 404);
            }

            return GenericResponse<List<LoanRequest>>.SuccessResponse(loans, 200, "Sucess fetching loans");
        }

        public async Task<GenericResponse<List<Loan>>> GetLoansByPoolAsync(string poolId)
        {
           var loans =  await _db.Loans.Where(x => x.PoolId == poolId).ToListAsync();

            if (loans == null)
            {
                return GenericResponse<List<Loan>>.FailResponse("No loans found in this pool", 404);
            }

            return GenericResponse<List<Loan>>.SuccessResponse(loans, 200, "Sucess fetching loans");
        }

        public async Task<GenericResponse<List<Loan>>> GetLoansByUserAsync(string userId)
        {
            var loans = await _db.Loans.Where(l => l.UserId == userId).ToListAsync();
            return GenericResponse<List<Loan>>.SuccessResponse(loans, 200);
        }

        public async Task<GenericResponse<Wallet>> GetOrCreateWalletAsync(string userId)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                wallet = new Wallet { UserId = userId, Balance = 0 };
                _db.Wallets.Add(wallet);
            }
            return GenericResponse<Wallet>.SuccessResponse(wallet, 200, "Sucess getting wallet");
        }
        public async Task<GenericResponse<List<Repayment>>> GetRepaymentsByLoanAsync(string loanId)
        {
            var loanRepayment = await _db.Repayments.Where(x => x.LoanId == loanId).ToListAsync();

            if (loanRepayment == null)
            {
                return GenericResponse<List<Repayment>>.FailResponse("no repayments for this loan", 404);

            }

            return GenericResponse<List<Repayment>>.SuccessResponse(loanRepayment, 200, $"{loanRepayment.Count} loans found ");
        }

        public async Task<GenericResponse<List<Repayment>>> GetRepaymentsByUserAsync(string userId)
        {
           var userLoans = await _db.Repayments.Where(x => x.Loan.UserId == userId).ToListAsync();

            if (userLoans == null)
            {
                return GenericResponse<List<Repayment>>.FailResponse("no repayments for this loan", 404);
            }

            return GenericResponse<List<Repayment>>.SuccessResponse(userLoans, 200, $"{userLoans.Count} loans found ");
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
