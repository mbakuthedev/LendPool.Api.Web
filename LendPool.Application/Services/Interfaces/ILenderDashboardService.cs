using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILenderDashboardService
    {
        Task<List<FundedLoanDto>> GetFundedLoansAsync(string lenderId);
        Task<LenderEarningsResponse> GetRepaymentsAndEarningsAsync(string lenderId);
        Task<List<PoolPerformanceDto>> GetPoolPerformanceAsync(string lenderId);
    }
} 