using System.Threading.Tasks;
using System.Collections.Generic;

namespace LendPool.Application.Services.Interfaces
{
    public interface IBorrowerAnalyticsService
    {
        Task<decimal> GetTotalBorrowedAsync(string userId);
        Task<decimal> GetCurrentLoanAsync(string userId);
        Task<decimal> GetOutstandingBalanceAsync(string userId);
        Task<string> GetNextPaymentDateAsync(string userId);
        Task<List<MonthAmountDto>> GetLoanByMonthAsync(string userId);
        Task<List<MonthAmountDto>> GetRepaymentsByMonthAsync(string userId);
        Task<RepaymentHistoryResponse> GetRepaymentHistoryAsync(string userId, int page, int pageSize);
    }

    public class MonthAmountDto
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class RepaymentHistoryItem
    {
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    public class RepaymentHistoryResponse
    {
        public List<RepaymentHistoryItem> Data { get; set; }
        public int Total { get; set; }
    }
} 