using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class ReconciliationRepository : IReconciliationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReconciliationRepository> _logger;

        public ReconciliationRepository(ApplicationDbContext context, ILogger<ReconciliationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GenericResponse<LoanReconciliation>> CreateReconciliationAsync(LoanReconciliation reconciliation)
        {
            try
            {
                _logger.LogInformation("Creating reconciliation for loan {LoanId}", reconciliation.LoanId);
                
                await _context.LoanReconciliations.AddAsync(reconciliation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created reconciliation {ReconciliationId}", reconciliation.Id);
                return GenericResponse<LoanReconciliation>.SuccessResponse(reconciliation, 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reconciliation for loan {LoanId}", reconciliation.LoanId);
                return GenericResponse<LoanReconciliation>.FailResponse("Failed to create reconciliation", 500);
            }
        }

        public async Task<GenericResponse<LoanReconciliation>> GetReconciliationByIdAsync(string reconciliationId)
        {
            try
            {
                var reconciliation = await _context.LoanReconciliations
                    .Include(r => r.Loan)
                    .Include(r => r.Lender)
                    .Include(r => r.Borrower)
                    .Include(r => r.ReconciliationItems)
                    .FirstOrDefaultAsync(r => r.Id == reconciliationId);

                if (reconciliation == null)
                {
                    return GenericResponse<LoanReconciliation>.FailResponse("Reconciliation not found", 404);
                }

                return GenericResponse<LoanReconciliation>.SuccessResponse(reconciliation, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation {ReconciliationId}", reconciliationId);
                return GenericResponse<LoanReconciliation>.FailResponse("Failed to retrieve reconciliation", 500);
            }
        }

        public async Task<GenericResponse<LoanReconciliation>> GetReconciliationByLoanAsync(string loanId)
        {
            try
            {
                var reconciliation = await _context.LoanReconciliations
                    .Include(r => r.Loan)
                    .Include(r => r.Lender)
                    .Include(r => r.Borrower)
                    .Include(r => r.ReconciliationItems)
                    .FirstOrDefaultAsync(r => r.LoanId == loanId);

                return GenericResponse<LoanReconciliation>.SuccessResponse(reconciliation, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation for loan {LoanId}", loanId);
                return GenericResponse<LoanReconciliation>.FailResponse("Failed to retrieve reconciliation", 500);
            }
        }

        public async Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByLoanAsync(string loanId)
        {
            try
            {
                var reconciliations = await _context.LoanReconciliations
                    .Include(r => r.Loan)
                    .Include(r => r.Lender)
                    .Include(r => r.Borrower)
                    .Include(r => r.ReconciliationItems)
                    .Where(r => r.LoanId == loanId)
                    .ToListAsync();

                return GenericResponse<List<LoanReconciliation>>.SuccessResponse(reconciliations, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for loan {LoanId}", loanId);
                return GenericResponse<List<LoanReconciliation>>.FailResponse("Failed to retrieve reconciliations", 500);
            }
        }

        public async Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByLenderAsync(string lenderId)
        {
            try
            {
                var reconciliations = await _context.LoanReconciliations
                    .Include(r => r.Loan)
                    .Include(r => r.Lender)
                    .Include(r => r.Borrower)
                    .Include(r => r.ReconciliationItems)
                    .Where(r => r.LenderId == lenderId)
                    .ToListAsync();

                return GenericResponse<List<LoanReconciliation>>.SuccessResponse(reconciliations, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for lender {LenderId}", lenderId);
                return GenericResponse<List<LoanReconciliation>>.FailResponse("Failed to retrieve reconciliations", 500);
            }
        }

        public async Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByBorrowerAsync(string borrowerId)
        {
            try
            {
                var reconciliations = await _context.LoanReconciliations
                    .Include(r => r.Loan)
                    .Include(r => r.Lender)
                    .Include(r => r.Borrower)
                    .Include(r => r.ReconciliationItems)
                    .Where(r => r.BorrowerId == borrowerId)
                    .ToListAsync();

                return GenericResponse<List<LoanReconciliation>>.SuccessResponse(reconciliations, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for borrower {BorrowerId}", borrowerId);
                return GenericResponse<List<LoanReconciliation>>.FailResponse("Failed to retrieve reconciliations", 500);
            }
        }

        public async Task<GenericResponse<LoanReconciliation>> UpdateReconciliationAsync(LoanReconciliation reconciliation)
        {
            try
            {
                _logger.LogInformation("Updating reconciliation {ReconciliationId}", reconciliation.Id);
                
                _context.LoanReconciliations.Update(reconciliation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated reconciliation {ReconciliationId}", reconciliation.Id);
                return GenericResponse<LoanReconciliation>.SuccessResponse(reconciliation, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reconciliation {ReconciliationId}", reconciliation.Id);
                return GenericResponse<LoanReconciliation>.FailResponse("Failed to update reconciliation", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationItem>> CreateReconciliationItemAsync(ReconciliationItem item)
        {
            try
            {
                _logger.LogInformation("Creating reconciliation item for reconciliation {ReconciliationId}", item.ReconciliationId);
                
                await _context.ReconciliationItems.AddAsync(item);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created reconciliation item {ItemId}", item.Id);
                return GenericResponse<ReconciliationItem>.SuccessResponse(item, 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reconciliation item for reconciliation {ReconciliationId}", item.ReconciliationId);
                return GenericResponse<ReconciliationItem>.FailResponse("Failed to create reconciliation item", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationItem>> GetReconciliationItemByIdAsync(string itemId)
        {
            try
            {
                var item = await _context.ReconciliationItems
                    .Include(i => i.Reconciliation)
                    .Include(i => i.FundUsage)
                    .Include(i => i.Disbursement)
                    .FirstOrDefaultAsync(i => i.Id == itemId);

                if (item == null)
                {
                    return GenericResponse<ReconciliationItem>.FailResponse("Reconciliation item not found", 404);
                }

                return GenericResponse<ReconciliationItem>.SuccessResponse(item, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation item {ItemId}", itemId);
                return GenericResponse<ReconciliationItem>.FailResponse("Failed to retrieve reconciliation item", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationItem>> GetReconciliationItemByFundUsageAsync(string fundUsageId)
        {
            try
            {
                var item = await _context.ReconciliationItems
                    .Include(i => i.Reconciliation)
                    .Include(i => i.FundUsage)
                    .Include(i => i.Disbursement)
                    .FirstOrDefaultAsync(i => i.FundUsageId == fundUsageId);

                return GenericResponse<ReconciliationItem>.SuccessResponse(item, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation item for fund usage {FundUsageId}", fundUsageId);
                return GenericResponse<ReconciliationItem>.FailResponse("Failed to retrieve reconciliation item", 500);
            }
        }

        public async Task<GenericResponse<List<ReconciliationItem>>> GetReconciliationItemsAsync(string reconciliationId)
        {
            try
            {
                var items = await _context.ReconciliationItems
                    .Include(i => i.Reconciliation)
                    .Include(i => i.FundUsage)
                    .Include(i => i.Disbursement)
                    .Where(i => i.ReconciliationId == reconciliationId)
                    .ToListAsync();

                return GenericResponse<List<ReconciliationItem>>.SuccessResponse(items, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation items for reconciliation {ReconciliationId}", reconciliationId);
                return GenericResponse<List<ReconciliationItem>>.FailResponse("Failed to retrieve reconciliation items", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationItem>> UpdateReconciliationItemAsync(ReconciliationItem item)
        {
            try
            {
                _logger.LogInformation("Updating reconciliation item {ItemId}", item.Id);
                
                _context.ReconciliationItems.Update(item);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated reconciliation item {ItemId}", item.Id);
                return GenericResponse<ReconciliationItem>.SuccessResponse(item, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reconciliation item {ItemId}", item.Id);
                return GenericResponse<ReconciliationItem>.FailResponse("Failed to update reconciliation item", 500);
            }
        }
    }
} 