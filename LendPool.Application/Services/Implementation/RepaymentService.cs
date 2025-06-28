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
    public class RepaymentService : IRepaymentService
    {
        private readonly ILendpoolRepository _poolRepository;
        private readonly ILoanRepository _loanRepository;
        private IRepaymentRepository _repaymentRepo;
        private readonly IWalletRepository _walletRepository;
        private ILogger<RepaymentService> _logger;

        public RepaymentService(ILendpoolRepository poolRepository, ILoanRepository loanRepository, IWalletRepository walletRepository, IRepaymentRepository repaymentRepo, ILogger<RepaymentService> logger)
        {
            _poolRepository = poolRepository;
            _loanRepository = loanRepository;
            _walletRepository = walletRepository;
            _logger = logger;
            _repaymentRepo = repaymentRepo;
        }

        public async Task<GenericResponse<Repayment>> AddRepaymentAsync(RepaymentDto dto)
        {
            try
            {
                var loan = await _loanRepository.GetLoanByIdAsync(dto.LoanId);
                if (loan.Data == null)
                    return GenericResponse<Repayment>.FailResponse("Loan not found.", 404);

                var userWallet = await _loanRepository.GetOrCreateWalletAsync(loan.Data.UserId);
                if (userWallet.Data == null)
                    return GenericResponse<Repayment>.FailResponse("User wallet not found.", 404);

                var loanEntity = loan.Data;
                var wallet = userWallet.Data;
                var paymentDate = DateTime.UtcNow;
                var dueDate = loanEntity.DueDate;

                // Calculate late fee
                var lateFee = CalculateLateFee(dueDate, paymentDate, dto.Amount);
                var isLate = lateFee > 0;

                var totalToDeduct = dto.Amount + lateFee;

                if (wallet.Balance < totalToDeduct)
                    return GenericResponse<Repayment>.FailResponse("Insufficient wallet balance.", 400);

                // Deduct from wallet
                wallet.Balance -= totalToDeduct;

                // Update loan balance
                loanEntity.TotalRepaid += dto.Amount;
                var remainingBalance = loanEntity.Amount - loanEntity.TotalRepaid;
                var isFullyRepaid = remainingBalance <= 0;

                if (isFullyRepaid)
                {
                    loanEntity.LoanStatus = LoanStatus.Repaid.ToString();
                    loanEntity.IsActive = false;
                }

                // Create repayment
                var repayment = new Repayment
                {
                    Id = Guid.NewGuid().ToString(),
                    LoanId = loanEntity.Id,
                    AmountPaid = dto.Amount,
                    PaymentDate = paymentDate,
                    LateFee = lateFee,
                    IsLate = isLate,
                    RemainingLoanBalance = remainingBalance,
                    IsFullyRepaid = isFullyRepaid,
                    LenderpoolId = loanEntity.PoolId,
                    TransactionReference = Guid.NewGuid().ToString(),
                    PaymentChannel = dto.PaymentChannel ?? "Wallet"
                };

                // Record wallet transaction
                var walletTx = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = totalToDeduct,
                    Type = TransactionType.Debit.ToString(),
                    Description = $"Loan repayment for {loanEntity.Id} (Amount: {dto.Amount}, Late Fee: {lateFee})",
                    Reference = repayment.TransactionReference
                };

                wallet.Transactions.Add(walletTx);

                // Persist
                await _repaymentRepo.AddRepayment(repayment);
     

                _logger.LogInformation("Repayment of {Amount} (Late fee: {LateFee}) made for loan {LoanId}", dto.Amount, lateFee, loanEntity.Id);

                return GenericResponse<Repayment>.SuccessResponse(repayment, 200, "Repayment recorded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding repayment for loan {LoanId}", dto.LoanId);
                return GenericResponse<Repayment>.FailResponse("Error recording repayment.", 500);
            }
        }


      
        /// <summary>
        /// Get a client repayment and potential earnings
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public async Task<GenericResponse<IEnumerable<RepaymentDto>>> GetRepaymentsAndEarningsAsync(string poolId)
        {
            var repayments = await _poolRepository.GetRepaymentsAsync(poolId);
            var repaymentsData = repayments?.Data;

            if (repayments == null || repaymentsData == null || !repaymentsData.Any())
            {
                return GenericResponse<IEnumerable<RepaymentDto>>.FailResponse("No active repayments found for this pool.", 404);
            }

            var repaymentDtos = repaymentsData.Select(repayment =>
            {
                var loan = repayment.Loan;
                var interestRate = loan?.InterestRate ?? 0;
                var interestEarned = CalculateInterestEarned(repayment.AmountPaid, interestRate);
                var principalPaid = repayment.AmountPaid - interestEarned;
                var dueDate = loan?.DueDate ?? DateTime.MinValue;
                var isLate = repayment.PaymentDate > dueDate;
                var lateFee = isLate ? CalculateLateFee(dueDate, repayment.PaymentDate, repayment.AmountPaid) : 0;
                var remainingBalance = loan != null
                    ? CalculateRemainingLoanBalance(loan.Amount, loan.TotalRepaid + repayment.AmountPaid)
                    : 0;

                return new RepaymentDto
                {
                    RepaymentId = repayment.Id,
                    LoanId = repayment.LoanId,
                    UserId = loan?.UserId ?? "Unknown",
                    PoolId = repayment.LenderpoolId,
                    Amount = repayment.AmountPaid,
                    InterestRate = interestRate,
                    InterestEarned = interestEarned,
                    PrincipalPaid = principalPaid,
                    RemainingLoanBalance = remainingBalance,
                    StartDate = repayment.PaymentDate,
                    DueDate = dueDate,
                    CreatedAt = repayment.DateCreated,
                    IsLate = isLate,
                    LateFee = lateFee,
                    IsFullyRepaid = remainingBalance == 0,
                    TransactionReference = repayment.TransactionReference,
                    PaymentChannel = repayment.PaymentChannel
                };
            });

            return GenericResponse<IEnumerable<RepaymentDto>>.SuccessResponse(repaymentDtos, 200, "Successfully fetched repayments and earnings.");
        }

        /// <summary>
        /// Calculate interest earned based on the amount paid and interest rate.
        /// </summary>
        /// <param name="amountPaid"></param>
        /// <param name="interestRate"></param>
        /// <returns></returns>
        public decimal CalculateInterestEarned(decimal amountPaid, decimal interestRate)
        {
            return amountPaid * (interestRate / 100);
        }

        /// <summary>
        /// Calculate late fee based on the due date, payment date, and base amount. 
        /// it incures a 1% daily late fee on the base amount for each day late.
        /// </summary>
        /// <param name="dueDate"></param>
        /// <param name="paymentDate"></param>
        /// <param name="baseAmount"></param>
        /// <returns></returns>
        public decimal CalculateLateFee(DateTime dueDate, DateTime paymentDate, decimal baseAmount)
        {
            if (paymentDate <= dueDate)
                return 0m;

            var daysLate = (paymentDate.Date - dueDate.Date).Days;
            const decimal penaltyRatePerDay = 0.01m; // 1% per day

            var lateFee = baseAmount * penaltyRatePerDay * daysLate;
            return Math.Round(lateFee, 2); // optional rounding
        }



        /// <summary>
        /// Calculate the remaining loan balance after repayments.
        /// </summary>
        /// <param name="totalLoanAmount"></param>
        /// <param name="totalRepaid"></param>
        /// <returns></returns>
        public decimal CalculateRemainingLoanBalance(decimal totalLoanAmount, decimal totalRepaid)
        {
            return Math.Max(0, totalLoanAmount - totalRepaid);
        }
    }

}
