using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Domain.Models;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IPoolTenorRepository : IBaseRepository<PoolTenor>
    {
        Task<PoolTenor> GetPoolTenorByPoolIdAsync(string poolId);
        Task<IEnumerable<PoolTenor>> GetPoolTenorsByStatusAsync(string status);
        Task<PoolTenor> AddPoolTenorAsync(PoolTenor poolTenor);
        Task<PoolTenor> UpdatePoolTenorAsync(PoolTenor poolTenor);
        Task<bool> UpdatePoolTenorStatusAsync(string poolId, string status);
        Task<decimal> GetTotalProfitByPoolIdAsync(string poolId);
        Task<decimal> GetTotalLossByPoolIdAsync(string poolId);
    }
} 