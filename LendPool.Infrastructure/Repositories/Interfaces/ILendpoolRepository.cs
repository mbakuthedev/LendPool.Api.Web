using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface ILendpoolRepository 
    {
        Task<bool> AddUserToPoolAsync(LenderPoolMembership lenderPoolMembership);
      //  Task<bool> AddUserToPoolAsync(string userId, string poolId);
        Task<bool> IsUserInPoolAsync(string userId, string poolId);

        Task<GenericResponse<LenderPool>> GetPoolById(string poolId);
        Task<GenericResponse<PoolSummaryDto>> GetPoolSummaryAsync(string poolId);
        Task<GenericResponse<bool>> RecordWithdrawalAsync(string poolId, string userId, decimal amount);
        Task<decimal> GetAvailableBalanceAsync(string poolId, string userId);
        Task<GenericResponse<LenderPool>> CreateAsync(LenderPool pool);
        Task<GenericResponse<LenderPool>> GetByIdAsync(string id);
        Task<GenericResponse<IEnumerable<LenderPool>>> GetAllAsync();
        Task<GenericResponse<IEnumerable<LoanDto>>> GetActiveLoansByPoolAsync(string poolId);
        Task<GenericResponse<decimal>> GetTotalEarningsAsync(string poolId);
        Task<GenericResponse<IEnumerable<Repayment>>> GetRepaymentsAsync(string poolId);
        Task<GenericResponse<bool>> ContributeAsync(PoolContribution contribution);
        Task<GenericResponse<bool>> WithdrawAsync(string poolId, decimal amount);
        Task<LenderPool> UpdateLenderPool(LenderPool pool);
    }
}
