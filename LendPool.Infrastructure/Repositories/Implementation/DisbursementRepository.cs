using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class DisbursementRepository : BaseRepository<Disbursement>, IDisbursementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DisbursementRepository> _logger;

        public DisbursementRepository(ApplicationDbContext context, ILogger<DisbursementRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Disbursement>> GetDisbursementsByLoanIdAsync(string loanId)
        {
            _logger.LogInformation("Querying disbursements for LoanId: {LoanId}", loanId);
            try
            {
                var result = await _context.Disbursements
                    .Where(d => d.LoanId == loanId)
                    .Include(d => d.FundUsages)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} disbursements for LoanId: {LoanId}", result.Count, loanId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying disbursements for LoanId: {LoanId}", loanId);
                throw;
            }
        }

        public async Task<Disbursement> GetDisbursementWithFundUsagesAsync(string id)
        {
            _logger.LogInformation("Querying disbursement with fund usages for DisbursementId: {DisbursementId}", id);
            try
            {
                var result = await _context.Disbursements
                    .Include(d => d.FundUsages)
                    .FirstOrDefaultAsync(d => d.Id == id);
                if (result == null)
                {
                    _logger.LogWarning("Disbursement not found: {DisbursementId}", id);
                }
                else
                {
                    _logger.LogInformation("Disbursement found: {DisbursementId}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying disbursement with fund usages for DisbursementId: {DisbursementId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<FundUsage>> GetFundUsagesByDisbursementIdAsync(string disbursementId)
        {
            _logger.LogInformation("Querying fund usages for DisbursementId: {DisbursementId}", disbursementId);
            try
            {
                var result = await _context.FundUsages
                    .Where(fu => fu.DisbursementId == disbursementId)
                    .ToListAsync();
                _logger.LogInformation("Found {Count} fund usages for DisbursementId: {DisbursementId}", result.Count, disbursementId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying fund usages for DisbursementId: {DisbursementId}", disbursementId);
                throw;
            }
        }

        public async Task<FundUsage> AddFundUsageAsync(FundUsage fundUsage)
        {
            _logger.LogInformation("Adding fund usage for DisbursementId: {DisbursementId}, BorrowerId: {BorrowerId}, AmountUsed: {AmountUsed}", fundUsage.DisbursementId, fundUsage.BorrowerId, fundUsage.AmountUsed);
            try
            {
                await _context.FundUsages.AddAsync(fundUsage);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Fund usage added: {FundUsageId} for Disbursement {DisbursementId}", fundUsage.Id, fundUsage.DisbursementId);
                return fundUsage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding fund usage for DisbursementId: {DisbursementId}", fundUsage.DisbursementId);
                throw;
            }
        }

        public async Task<GenericResponse<List<Disbursement>>> GetDisbursementsByLoanAsync(string loanId)
        {
            var disbursements = await _context.Disbursements
                .Where(d => d.LoanId == loanId)
                .Include(d => d.FundUsages)
                .ToListAsync();
            return GenericResponse<List<Disbursement>>.SuccessResponse(disbursements, 200);
        }

        public async Task<GenericResponse<List<FundUsage>>> GetFundUsagesByLoanAsync(string loanId)
        {
            var disbursements = await _context.Disbursements
                .Where(d => d.LoanId == loanId)
                .Include(d => d.FundUsages)
                .ToListAsync();
            var fundUsages = disbursements.SelectMany(d => d.FundUsages).ToList();
            return GenericResponse<List<FundUsage>>.SuccessResponse(fundUsages, 200);
        }

        public async Task<GenericResponse<FundUsage>> GetFundUsageByIdAsync(string fundUsageId)
        {
            var fundUsage = await _context.FundUsages
                .FirstOrDefaultAsync(f => f.Id == fundUsageId);
            if (fundUsage == null)
                return GenericResponse<FundUsage>.FailResponse("Fund usage not found", 404);
            return GenericResponse<FundUsage>.SuccessResponse(fundUsage, 200);
        }

        public async Task<GenericResponse<FundUsage>> UpdateFundUsageAsync(FundUsage fundUsage)
        {
            _context.FundUsages.Update(fundUsage);
            var saved = await _context.SaveChangesAsync() > 0;
            if (!saved)
                return GenericResponse<FundUsage>.FailResponse("Failed to update fund usage", 500);
            return GenericResponse<FundUsage>.SuccessResponse(fundUsage, 200);
        }
    }
} 