using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class PoolTenorRepository : BaseRepository<PoolTenor>, IPoolTenorRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PoolTenorRepository> _logger;

        public PoolTenorRepository(ApplicationDbContext context, ILogger<PoolTenorRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PoolTenor> GetPoolTenorByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Querying pool tenor for PoolId: {PoolId}", poolId);
            try
            {
                var result = await _context.PoolTenors
                    .Include(pt => pt.Loans)
                    .Include(pt => pt.LenderInvestments)
                    .FirstOrDefaultAsync(pt => pt.PoolId == poolId);
                if (result == null)
                {
                    _logger.LogWarning("Pool tenor not found: {PoolId}", poolId);
                }
                else
                {
                    _logger.LogInformation("Pool tenor found: {PoolId}", poolId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying pool tenor for PoolId: {PoolId}", poolId);
                throw;
            }
        }

        public async Task<IEnumerable<PoolTenor>> GetPoolTenorsByStatusAsync(string status)
        {
            _logger.LogInformation("Querying pool tenors by status: {Status}", status);
            try
            {
                var result = await _context.PoolTenors
                    .Where(pt => pt.Status.ToString() == status)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} pool tenors with status: {Status}", result.Count, status);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying pool tenors by status: {Status}", status);
                throw;
            }
        }

        public async Task<PoolTenor> AddPoolTenorAsync(PoolTenor poolTenor)
        {
            _logger.LogInformation("Adding pool tenor for PoolId: {PoolId}, Duration: {Duration} months", 
                poolTenor.PoolId, poolTenor.DurationInMonths);
            try
            {
                await _context.PoolTenors.AddAsync(poolTenor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pool tenor added: {TenorId} for Pool {PoolId}", 
                    poolTenor.Id, poolTenor.PoolId);
                return poolTenor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pool tenor for PoolId: {PoolId}", poolTenor.PoolId);
                throw;
            }
        }

        public async Task<PoolTenor> UpdatePoolTenorAsync(PoolTenor poolTenor)
        {
            _logger.LogInformation("Updating pool tenor: {TenorId}", poolTenor.Id);
            try
            {
                _context.PoolTenors.Update(poolTenor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pool tenor updated: {TenorId}", poolTenor.Id);
                return poolTenor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pool tenor: {TenorId}", poolTenor.Id);
                throw;
            }
        }

        public async Task<bool> UpdatePoolTenorStatusAsync(string poolId, string status)
        {
            _logger.LogInformation("Updating pool tenor status for PoolId: {PoolId} to {Status}", poolId, status);
            try
            {
                var poolTenor = await _context.PoolTenors.FirstOrDefaultAsync(pt => pt.PoolId == poolId);
                if (poolTenor == null)
                {
                    _logger.LogWarning("Pool tenor not found: {PoolId}", poolId);
                    return false;
                }

               
                    poolTenor.Status = status;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Pool tenor status updated: {PoolId} to {Status}", poolId, status);
                    return true;
                
          
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pool tenor status for PoolId: {PoolId}", poolId);
                throw;
            }
        }

        public async Task<decimal> GetTotalProfitByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Calculating total profit for PoolId: {PoolId}", poolId);
            try
            {
                var total = await _context.PoolTenors
                    .Where(pt => pt.PoolId == poolId)
                    .SumAsync(pt => pt.TotalProfit);
                _logger.LogInformation("Total profit for PoolId {PoolId}: {Total}", poolId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total profit for PoolId: {PoolId}", poolId);
                throw;
            }
        }

        public async Task<decimal> GetTotalLossByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Calculating total loss for PoolId: {PoolId}", poolId);
            try
            {
                var total = await _context.PoolTenors
                    .Where(pt => pt.PoolId == poolId)
                    .SumAsync(pt => pt.TotalLoss);
                _logger.LogInformation("Total loss for PoolId {PoolId}: {Total}", poolId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total loss for PoolId: {PoolId}", poolId);
                throw;
            }
        }
    }
} 