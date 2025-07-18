using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Domain.Models;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface ILenderInvestmentRepository : IBaseRepository<LenderInvestment>
    {
        Task<LenderInvestment> GetInvestmentByIdAsync(string investmentId);
        Task<IEnumerable<LenderInvestment>> GetInvestmentsByPoolIdAsync(string poolId);
        Task<IEnumerable<LenderInvestment>> GetInvestmentsByLenderIdAsync(string lenderId);
        Task<LenderInvestment> AddInvestmentAsync(LenderInvestment investment);
        Task<LenderInvestment> UpdateInvestmentAsync(LenderInvestment investment);
        Task<decimal> GetTotalInvestmentAmountByPoolIdAsync(string poolId);
    }
} 