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
    public class LenderInvestmentRepository : BaseRepository<LenderInvestment>, ILenderInvestmentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LenderInvestmentRepository> _logger;

        public LenderInvestmentRepository(ApplicationDbContext context, ILogger<LenderInvestmentRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LenderInvestment> GetInvestmentByIdAsync(string investmentId)
        {
            _logger.LogInformation("Querying investment by Id: {InvestmentId}", investmentId);
            try
            {
                var result = await _context.LenderInvestments
                    .FirstOrDefaultAsync(i => i.Id == investmentId);
                if (result == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", investmentId);
                }
                else
                {
                    _logger.LogInformation("Investment found: {InvestmentId}", investmentId);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying investment by Id: {InvestmentId}", investmentId);
                throw;
            }
        }

        public async Task<IEnumerable<LenderInvestment>> GetInvestmentsByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Querying investments for PoolId: {PoolId}", poolId);
            try
            {
                var result = await _context.LenderInvestments
                    .Where(i => i.PoolId == poolId)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} investments for PoolId: {PoolId}", result.Count, poolId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying investments for PoolId: {PoolId}", poolId);
                throw;
            }
        }

        public async Task<IEnumerable<LenderInvestment>> GetInvestmentsByLenderIdAsync(string lenderId)
        {
            _logger.LogInformation("Querying investments for LenderId: {LenderId}", lenderId);
            try
            {
                var result = await _context.LenderInvestments
                    .Where(i => i.LenderId == lenderId)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} investments for LenderId: {LenderId}", result.Count, lenderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying investments for LenderId: {LenderId}", lenderId);
                throw;
            }
        }

        public async Task<LenderInvestment> AddInvestmentAsync(LenderInvestment investment)
        {
            _logger.LogInformation("Adding investment for LenderId: {LenderId}, PoolId: {PoolId}, Amount: {Amount}", 
                investment.LenderId, investment.PoolId, investment.InvestmentAmount);
            try
            {
                await _context.LenderInvestments.AddAsync(investment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Investment added: {InvestmentId} for Lender {LenderId}", 
                    investment.Id, investment.LenderId);
                return investment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding investment for LenderId: {LenderId}", investment.LenderId);
                throw;
            }
        }

        public async Task<LenderInvestment> UpdateInvestmentAsync(LenderInvestment investment)
        {
            _logger.LogInformation("Updating investment: {InvestmentId}", investment.Id);
            try
            {
                _context.LenderInvestments.Update(investment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Investment updated: {InvestmentId}", investment.Id);
                return investment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating investment: {InvestmentId}", investment.Id);
                throw;
            }
        }

        public async Task<decimal> GetTotalInvestmentAmountByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Calculating total investment amount for PoolId: {PoolId}", poolId);
            try
            {
                var total = await _context.LenderInvestments
                    .Where(i => i.PoolId == poolId && i.Status == InvestmentStatus.Active.ToString())
                    .SumAsync(i => i.InvestmentAmount);
                _logger.LogInformation("Total investment amount for PoolId {PoolId}: {Total}", poolId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total investment amount for PoolId: {PoolId}", poolId);
                throw;
            }
        }
    }
} 