using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IDisbursementRepository : IBaseRepository<Disbursement>
    {
        Task<IEnumerable<Disbursement>> GetDisbursementsByLoanIdAsync(string loanId);
        Task<Disbursement> GetDisbursementWithFundUsagesAsync(string id);
        Task<IEnumerable<FundUsage>> GetFundUsagesByDisbursementIdAsync(string disbursementId);
        Task<FundUsage> AddFundUsageAsync(FundUsage fundUsage);
        Task<GenericResponse<List<Disbursement>>> GetDisbursementsByLoanAsync(string loanId);
        Task<GenericResponse<List<FundUsage>>> GetFundUsagesByLoanAsync(string loanId);
        Task<GenericResponse<FundUsage>> GetFundUsageByIdAsync(string fundUsageId);
        Task<GenericResponse<FundUsage>> UpdateFundUsageAsync(FundUsage fundUsage);
    }
} 