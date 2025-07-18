using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IDisbursementService
    {
        Task<GenericResponse<DisbursementDto>> CreateDisbursementAsync(CreateDisbursementDto dto);
        Task<GenericResponse<IEnumerable<DisbursementDto>>> GetDisbursementsByLoanIdAsync(string loanId);
        Task<GenericResponse<DisbursementDto>> GetDisbursementByIdAsync(string id);
        Task<GenericResponse<FundUsageDto>> LogFundUsageAsync(CreateFundUsageDto dto);
        Task<GenericResponse<IEnumerable<FundUsageDto>>> GetFundUsagesByDisbursementIdAsync(string disbursementId);
    }
} 