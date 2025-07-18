using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILenderDashboardService
    {
        Task<GenericResponse<List<FundedLoanDto>>> GetFundedLoansAsync(string lenderId);
        Task<GenericResponse<LenderEarningsResponse>> GetRepaymentsAndEarningsAsync(string lenderId);
        Task<GenericResponse<List<PoolPerformanceDto>>> GetPoolPerformanceAsync(string lenderId);
    }
} 