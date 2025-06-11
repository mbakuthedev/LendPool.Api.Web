using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;

namespace LendPool.Application.Services.Implementation
{
    public class RepaymentService : IRepaymentService
    {
        private readonly ILendpoolRepository _poolRepository;

        public RepaymentService(ILendpoolRepository poolRepository)
        {
            _poolRepository = poolRepository;
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
            var daysLate = (paymentDate - dueDate).Days;
            var penaltyRatePerDay = 0.01m; // 1% daily late fee
            return baseAmount * penaltyRatePerDay * daysLate;
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
