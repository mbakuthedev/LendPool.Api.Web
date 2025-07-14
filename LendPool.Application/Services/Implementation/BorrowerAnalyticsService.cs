using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Application.Services.Implementation
{
    public class BorrowerAnalyticsService : IBorrowerAnalyticsService
    {
        public Task<decimal> GetTotalBorrowedAsync(string userId)
            => Task.FromResult(72000.38m); // TODO: Implement real logic

        public Task<decimal> GetCurrentLoanAsync(string userId)
            => Task.FromResult(10000m); // TODO: Implement real logic

        public Task<decimal> GetOutstandingBalanceAsync(string userId)
            => Task.FromResult(9156.28m); // TODO: Implement real logic

        public Task<string> GetNextPaymentDateAsync(string userId)
            => Task.FromResult("2025-06-31"); // TODO: Implement real logic

        public Task<List<MonthAmountDto>> GetLoanByMonthAsync(string userId)
            => Task.FromResult(new List<MonthAmountDto> {
                new MonthAmountDto { Month = "Jan", Amount = 10000 },
                new MonthAmountDto { Month = "Feb", Amount = 12000 },
                new MonthAmountDto { Month = "Mar", Amount = 15000 },
                new MonthAmountDto { Month = "Apr", Amount = 20000 },
                new MonthAmountDto { Month = "May", Amount = 5000 },
                new MonthAmountDto { Month = "Other", Amount = 5000 }
            });

        public Task<List<MonthAmountDto>> GetRepaymentsByMonthAsync(string userId)
            => Task.FromResult(new List<MonthAmountDto> {
                new MonthAmountDto { Month = "Jan", Amount = 5000 },
                new MonthAmountDto { Month = "Feb", Amount = 6000 },
                new MonthAmountDto { Month = "Mar", Amount = 8000 },
                new MonthAmountDto { Month = "Apr", Amount = 10000 },
                new MonthAmountDto { Month = "May", Amount = 2000 },
                new MonthAmountDto { Month = "Other", Amount = 3000 }
            });

        public Task<RepaymentHistoryResponse> GetRepaymentHistoryAsync(string userId, int page, int pageSize)
            => Task.FromResult(new RepaymentHistoryResponse {
                Data = new List<RepaymentHistoryItem> {
                    new RepaymentHistoryItem { Date = "2025-01-15", Amount = 5000, Status = "Paid" },
                    new RepaymentHistoryItem { Date = "2025-02-15", Amount = 6000, Status = "Paid" }
                },
                Total = 2
            });
    }
} 