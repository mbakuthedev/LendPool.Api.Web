using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IReconciliationRepository
    {
        Task<GenericResponse<LoanReconciliation>> CreateReconciliationAsync(LoanReconciliation reconciliation);
        Task<GenericResponse<LoanReconciliation>> GetReconciliationByIdAsync(string reconciliationId);
        Task<GenericResponse<LoanReconciliation>> GetReconciliationByLoanAsync(string loanId);
        Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByLoanAsync(string loanId);
        Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByLenderAsync(string lenderId);
        Task<GenericResponse<List<LoanReconciliation>>> GetReconciliationsByBorrowerAsync(string borrowerId);
        Task<GenericResponse<LoanReconciliation>> UpdateReconciliationAsync(LoanReconciliation reconciliation);
        
        Task<GenericResponse<ReconciliationItem>> CreateReconciliationItemAsync(ReconciliationItem item);
        Task<GenericResponse<ReconciliationItem>> GetReconciliationItemByIdAsync(string itemId);
        Task<GenericResponse<ReconciliationItem>> GetReconciliationItemByFundUsageAsync(string fundUsageId);
        Task<GenericResponse<List<ReconciliationItem>>> GetReconciliationItemsAsync(string reconciliationId);
        Task<GenericResponse<ReconciliationItem>> UpdateReconciliationItemAsync(ReconciliationItem item);
    }
} 