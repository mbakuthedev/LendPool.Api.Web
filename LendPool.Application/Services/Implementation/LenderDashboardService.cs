using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Application.Services.Implementation
{
    public class LenderDashboardService : ILenderDashboardService
    {
        public Task<List<FundedLoanDto>> GetFundedLoansAsync(string lenderId) =>
            Task.FromResult(new List<FundedLoanDto>
            {
                new FundedLoanDto { Borrower = "Jane Doe", Amount = 500000, Pool = "Pool A", Status = "active", Date = "2024-06-20" },
                new FundedLoanDto { Borrower = "John Smith", Amount = 200000, Pool = "Pool B", Status = "repaid", Date = "2024-05-15" }
            });

        public Task<LenderEarningsResponse> GetRepaymentsAndEarningsAsync(string lenderId)
        {
            // Simulate repayments using the Repayment model structure
            var repayments = new List<(string LoanId, decimal AmountPaid, string PaymentDate, bool IsFullyRepaid, bool IsLate)>
            {
                ("I1", 50000, "2024-07-01", true, false),
                ("I2", 200000, "2024-06-01", true, false),
                ("I3", 30000, "2024-05-01", false, true)
            };

            var repaymentDtos = new List<LenderRepaymentDto>();
            foreach (var r in repayments)
            {
                string status = r.IsFullyRepaid ? "paid" : (r.IsLate ? "late" : "pending");
                repaymentDtos.Add(new LenderRepaymentDto
                {
                    Loan = r.LoanId,
                    Amount = r.AmountPaid,
                    Date = r.PaymentDate,
                    Status = status
                });
            }

            return Task.FromResult(new LenderEarningsResponse
            {
                TotalEarnings = 250000,
                Repayments = repaymentDtos
            });
        }

        public Task<List<PoolPerformanceDto>> GetPoolPerformanceAsync(string lenderId) =>
            Task.FromResult(new List<PoolPerformanceDto>
            {
                new PoolPerformanceDto { Pool = "Pool A", PerformanceValue = 0.85m },
                new PoolPerformanceDto { Pool = "Pool B", PerformanceValue = 0.92m }
            });
    }
} 