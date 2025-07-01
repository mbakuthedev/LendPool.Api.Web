using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<LoanService> _logger;
        private readonly ILendpoolRepository _lenderpoolRepo;

        public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger, ILendpoolRepository lenderpoolRepo)
        {
            _loanRepository = loanRepository;
            _logger = logger;
            _lenderpoolRepo = lenderpoolRepo;
        }

        public async Task<GenericResponse<Loan>> AddLoanAsync(CreateLoanDto loanDto)
        {
            try
            {
                var loan = new Loan
                {
               
                    UserId = loanDto.UserId,
                    PoolId = loanDto.PoolId,
                    Amount = loanDto.Amount,
                    InterestRate = loanDto.InterestRate,
                    StartDate = loanDto.StartDate,
                    DueDate = loanDto.DueDate,
                    IsActive = true,
                    LoanStatus = LoanStatus.Active.ToString(),
                    TotalRepaid = 0
                };

                var response = await _loanRepository.AddLoanAsync(loan);

                if (response.Data == null)
                {
                    _logger.LogInformation("Loan could not be  created for user {UserId} in pool {PoolId}.", loanDto.UserId, loanDto.PoolId);
                    return GenericResponse<Loan>.FailResponse("Loan couldnt be created", 404);
                  
                }

                return GenericResponse<Loan>.SuccessResponse(loan, 201, "Sucess creating user");
         
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loan for user {UserId}", loanDto.UserId);
                return GenericResponse<Loan>.FailResponse("An error occurred while creating the loan.", 500);
            }
        }

        public async Task<GenericResponse<LoanRequestResponseDto>> SubmitLoanRequestAsync(string userId, LoanRequestDto dto)
        {
            if (dto.RequestedAmount <= 0)
                return GenericResponse<LoanRequestResponseDto>.FailResponse("Requested amount must be greater than 0.", 400);

            if (dto.DurationInMonths <= 0)
                return GenericResponse<LoanRequestResponseDto>.FailResponse("Tenure must be at least 1 month.", 400);

            if (string.IsNullOrWhiteSpace(dto.Purpose))
                return GenericResponse<LoanRequestResponseDto>.FailResponse("Purpose is required.", 400);

            if (string.IsNullOrWhiteSpace(dto.MatchedPoolId))
                return GenericResponse<LoanRequestResponseDto>.FailResponse("Pool ID is required.", 400);

            var startDate = DateTime.UtcNow;
            var dueDate = startDate.AddMonths(dto.DurationInMonths);

            var request = new LoanRequest
            {
                Id = Guid.NewGuid().ToString(),
                BorrowerId = userId,
                Amount = dto.RequestedAmount,
               DurationInMonths = dto.DurationInMonths,
                TenureInDays = (dueDate - startDate).Days,
                Purpose = dto.Purpose.Trim(),
                RequestStatus = LoanRequestStatus.Pending.ToString(),
                DateCreated = startDate,
                MatchedPoolId = dto.MatchedPoolId
            };

            try
            {
                var result = await _loanRepository.AddLoanRequestAsync(request);
              

                _logger.LogInformation("Loan request submitted by user {UserId} for pool {PoolId}", userId, dto.MatchedPoolId);

                var responseDto = new LoanRequestResponseDto
                {
                    Id = request.Id,
                    Amount = request.Amount,
                    TenureInDays = request.TenureInDays,
                    Purpose = request.Purpose,
                    RequestStatus = request.RequestStatus,
                    DateCreated = request.DateCreated,
                    MatchedPoolId = request.MatchedPoolId
                };

                return GenericResponse<LoanRequestResponseDto>.SuccessResponse(responseDto, 201, "Loan request submitted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting loan request for user {UserId}", userId);
                return GenericResponse<LoanRequestResponseDto>.FailResponse("Failed to submit loan request.");
            }
        }

        public async Task<GenericResponse<string>> ApproveLoanAsync(string lenderId,  ApproveLoanRequestDto dto)
        {
            var request = await _loanRepository.GetLoanRequestByIdAsync(dto.RequestId);
            if (request.Data == null)
                return GenericResponse<string>.FailResponse("Loan request not found", 404);

            var loanRequest = request.Data;

            // Check if lender already approved
            var alreadyApproved = await _loanRepository.GetApprovals(dto.RequestId, lenderId);
            if (alreadyApproved.Data)
                return GenericResponse<string>.FailResponse("You have already approved this request.", 400);

            // Add approval
            var approval = new LoanApproval
            {
                LoanRequestId = dto.RequestId,
                LenderId = lenderId
            };

            await _loanRepository.AddApprovalAsync(approval);

            // Check if enough approvals exist
            var approvalCount = await _loanRepository.GetNumberOfApprovals(dto.RequestId);

            if (approvalCount.Data >= 3 && loanRequest.RequestStatus != LoanRequestStatus.Approved.ToString())
            {
                loanRequest.RequestStatus = LoanRequestStatus.Approved.ToString();

                loanRequest.AdminComment = dto.Comment;

                // Fetch the pool to get the interest rate
                var pool = await _lenderpoolRepo.GetByIdAsync(loanRequest.MatchedPoolId);
             
                if (pool.Data == null)
                    return GenericResponse<string>.FailResponse("Matched pool not found.", 404);

                var loan = new Loan
                {
                    UserId = loanRequest.BorrowerId,
                    PoolId = loanRequest.MatchedPoolId,
                    Amount = loanRequest.Amount,
                    InterestRate = pool.Data.InterestRate,
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(loanRequest.TenureInDays),
                    LoanStatus = LoanStatus.Active.ToString(),
                    TotalRepaid = 0,
                    IsActive = true,
                    LoanRequestId = dto.RequestId,
                };

                await _loanRepository.AddLoanAsync(loan);
                await _loanRepository.SaveChangesAsync();

                return GenericResponse<string>.SuccessResponse("Loan approved and disbursed.", 200);
            }

            return GenericResponse<string>.SuccessResponse($"Approval recorded. ({approvalCount.Data}/3)", 200);
        }

       // add this to a transaction service
        public async Task<GenericResponse<Transaction>> AddTransactionAsync(Transaction transaction)
        {
            try
            {
                var result = await _loanRepository.AddTransactionAsync(transaction);

                if (result.Data == null)
                {
                    return GenericResponse<Transaction>.FailResponse( "Transaction failed to process.", 404);
                }

                _logger.LogInformation("Transaction added for user {UserId}.", transaction.UserId);

                return GenericResponse<Transaction>.SuccessResponse(result.Data, 200, "Transaction added.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording transaction.");
                return GenericResponse<Transaction>.FailResponse("Error recording transaction.", 500);
            }
        }

        public async Task<GenericResponse<List<LoanRequest>>> GetAllLoanRequestsAsync() =>
            await _loanRepository.GetAllLoanRequestsAsync();

        public async Task<GenericResponse<List<Loan>>> GetAllLoansAsync() =>
            await _loanRepository.GetAllLoansAsync();

        public async Task<GenericResponse<Loan>> GetLoanByIdAsync(string id) =>
            await _loanRepository.GetLoanByIdAsync(id);

        public async Task<GenericResponse<LoanRequest>> GetLoanRequestByIdAsync(string requestId) =>
            await _loanRepository.GetLoanRequestByIdAsync(requestId);

        public async Task<GenericResponse<List<LoanRequest>>> GetLoanRequestsByUserAsync(string userId) =>
            await _loanRepository.GetLoanRequestsByUserAsync(userId);

        public async Task<GenericResponse<List<Loan>>> GetLoansByPoolAsync(string poolId) =>
            await _loanRepository.GetLoansByPoolAsync(poolId);

        public async Task<GenericResponse<List<Loan>>> GetLoansByUserAsync(string userId) =>
            await _loanRepository.GetLoansByUserAsync(userId);

        public async Task<GenericResponse<Wallet>> GetOrCreateWalletAsync(string userId) =>
            await _loanRepository.GetOrCreateWalletAsync(userId);

        public async Task<GenericResponse<List<Repayment>>> GetRepaymentsByLoanAsync(string loanId) =>
            await _loanRepository.GetRepaymentsByLoanAsync(loanId);

        public async Task<GenericResponse<List<Repayment>>> GetRepaymentsByUserAsync(string userId) =>
            await _loanRepository.GetRepaymentsByUserAsync(userId);
    }
}
