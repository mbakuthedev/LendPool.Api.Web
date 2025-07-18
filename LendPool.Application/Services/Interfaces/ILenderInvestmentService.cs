using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILenderInvestmentService
    {
        Task<GenericResponse<LenderInvestmentDto>> CreateInvestmentAsync(CreateLenderInvestmentDto dto);
        Task<GenericResponse<LenderInvestmentDto>> GetInvestmentByIdAsync(string investmentId);
        Task<GenericResponse<IEnumerable<LenderInvestmentDto>>> GetInvestmentsByPoolIdAsync(string poolId);
        Task<GenericResponse<IEnumerable<LenderInvestmentDto>>> GetInvestmentsByLenderIdAsync(string lenderId);
        Task<GenericResponse<LenderInvestmentDto>> WithdrawInvestmentAsync(WithdrawInvestmentDto dto);
        Task<GenericResponse<decimal>> CalculateProfitShareAsync(string investmentId);
        Task<GenericResponse<decimal>> CalculateLossShareAsync(string investmentId);
        Task<GenericResponse<decimal>> CalculateEarlyWithdrawalPenaltyAsync(string investmentId);
        Task<GenericResponse<PoolTenorDto>> GetPoolTenorAsync(string poolId);
        Task<GenericResponse<bool>> UpdatePoolTenorStatusAsync(string poolId, string status);
    }
} 