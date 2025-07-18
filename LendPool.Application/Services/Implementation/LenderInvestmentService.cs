using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class LenderInvestmentService : ILenderInvestmentService
    {
        private readonly ILenderInvestmentRepository _investmentRepository;
        private readonly IPoolTenorRepository _tenorRepository;
        private readonly ILendpoolRepository _poolRepository;
        private readonly ILogger<LenderInvestmentService> _logger;

        public LenderInvestmentService(
            ILenderInvestmentRepository investmentRepository,
            IPoolTenorRepository tenorRepository,
            ILendpoolRepository poolRepository,
            ILogger<LenderInvestmentService> logger)
        {
            _investmentRepository = investmentRepository;
            _tenorRepository = tenorRepository;
            _poolRepository = poolRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<LenderInvestmentDto>> CreateInvestmentAsync(CreateLenderInvestmentDto dto)
        {
            _logger.LogInformation("Creating investment for LenderId: {LenderId}, PoolId: {PoolId}, Amount: {Amount}", 
                dto.LenderId, dto.PoolId, dto.InvestmentAmount);

            try
            {
                // Get pool to calculate percentage
                var pool = await _poolRepository.GetByIdAsync(dto.PoolId);
                if (pool == null)
                {
                    _logger.LogWarning("Pool not found: {PoolId}", dto.PoolId);
                    return GenericResponse<LenderInvestmentDto>.FailResponse("Pool not found", 404);
                }

                // Calculate investment percentage
                var totalPoolAmount = pool.Data?.TotalCapital;
                var investmentPercentage = (dto.InvestmentAmount / totalPoolAmount) * 100;

                var investment = new LenderInvestment
                {
                    LenderId = dto.LenderId,
                    PoolId = dto.PoolId,
                    InvestmentAmount = dto.InvestmentAmount,
                    InvestmentPercentage = investmentPercentage ?? 0,
                    InvestmentDate = DateTime.UtcNow,
                    Status = InvestmentStatus.Active.ToString(),
                    ProfitEarned = 0,
                    LossIncurred = 0,
                    WithdrawalAmount = 0,
                    IsEarlyWithdrawal = false,
                    EarlyWithdrawalPenalty = 0
                };

                var result = await _investmentRepository.AddInvestmentAsync(investment);
                _logger.LogInformation("Investment created: {InvestmentId} with {Percentage}% share", 
                    result.Id, investmentPercentage);

                return GenericResponse<LenderInvestmentDto>.SuccessResponse(MapToDto(result), 201, "Investment created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating investment for LenderId: {LenderId}", dto.LenderId);
                return GenericResponse<LenderInvestmentDto>.FailResponse($"Error creating investment: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<LenderInvestmentDto>> WithdrawInvestmentAsync(WithdrawInvestmentDto dto)
        {
            _logger.LogInformation("Processing withdrawal for InvestmentId: {InvestmentId}, Amount: {Amount}", 
                dto.InvestmentId, dto.WithdrawalAmount);

            try
            {
                var investment = await _investmentRepository.GetInvestmentByIdAsync(dto.InvestmentId);
                if (investment == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", dto.InvestmentId);
                    return GenericResponse<LenderInvestmentDto>.FailResponse("Investment not found", 404);
                }

                // Get pool tenor to check if it's early withdrawal
                var poolTenor = await _tenorRepository.GetPoolTenorByPoolIdAsync(investment.PoolId);
                var isEarlyWithdrawal = poolTenor != null && DateTime.UtcNow < poolTenor.ExpectedEndDate;

                decimal penalty = 0;
                decimal actualWithdrawalAmount = dto.WithdrawalAmount;

                if (isEarlyWithdrawal)
                {
                    // calculate withdrawal fee and serve penalty - lmao you got served
                    var penaltyResult = await CalculateEarlyWithdrawalPenaltyAsync(investment.Id);
                    penalty = penaltyResult.Data;
                    actualWithdrawalAmount = dto.WithdrawalAmount - penalty;
                    _logger.LogInformation("Early withdrawal penalty applied: {Penalty} for InvestmentId: {InvestmentId}", 
                        penalty, dto.InvestmentId);
                }

                // Update investment
                investment.WithdrawalDate = DateTime.UtcNow;
                investment.WithdrawalAmount = actualWithdrawalAmount;
                investment.Status = InvestmentStatus.Withdrawn.ToString();
                investment.WithdrawalReason = dto.WithdrawalReason;
                investment.IsEarlyWithdrawal = isEarlyWithdrawal;
                investment.EarlyWithdrawalPenalty = penalty;

                var updatedInvestment = await _investmentRepository.UpdateInvestmentAsync(investment);
                _logger.LogInformation("Investment withdrawn: {InvestmentId}, Actual amount: {Amount}, Penalty: {Penalty}", 
                    investment.Id, actualWithdrawalAmount, penalty);

                return GenericResponse<LenderInvestmentDto>.SuccessResponse(MapToDto(updatedInvestment), 200, "Investment withdrawn successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing investment: {InvestmentId}", dto.InvestmentId);
                return GenericResponse<LenderInvestmentDto>.FailResponse($"Error withdrawing investment: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<decimal>> CalculateProfitShareAsync(string investmentId)
        {
            _logger.LogInformation("Calculating profit share for InvestmentId: {InvestmentId}", investmentId);

            try
            {
                var investment = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
                if (investment == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", investmentId);
                    return GenericResponse<decimal>.FailResponse("Investment not found", 404);
                }

                var poolTenor = await _tenorRepository.GetPoolTenorByPoolIdAsync(investment.PoolId);
                if (poolTenor == null)
                {
                    _logger.LogWarning("Pool tenor not found for PoolId: {PoolId}", investment.PoolId);
                    return GenericResponse<decimal>.FailResponse("Pool tenor not found", 404);
                }

                // Calculate profit share based on investment percentage
                var profitShare = (poolTenor.TotalProfit * investment.InvestmentPercentage) / 100;

                _logger.LogInformation("Profit share calculated: {ProfitShare} for InvestmentId: {InvestmentId}", 
                    profitShare, investmentId);

                return GenericResponse<decimal>.SuccessResponse(profitShare, 200, "Profit share calculated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating profit share for InvestmentId: {InvestmentId}", investmentId);
                return GenericResponse<decimal>.FailResponse($"Error calculating profit share: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<decimal>> CalculateLossShareAsync(string investmentId)
        {
            _logger.LogInformation("Calculating loss share for InvestmentId: {InvestmentId}", investmentId);

            try
            {
                var investment = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
                if (investment == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", investmentId);
                    return GenericResponse<decimal>.FailResponse("Investment not found", 404);
                }

                var poolTenor = await _tenorRepository.GetPoolTenorByPoolIdAsync(investment.PoolId);
                if (poolTenor == null)
                {
                    _logger.LogWarning("Pool tenor not found for PoolId: {PoolId}", investment.PoolId);
                    return GenericResponse<decimal>.FailResponse("Pool tenor not found", 404);
                }

                // Calculate loss share based on investment percentage
                var lossShare = (poolTenor.TotalLoss * investment.InvestmentPercentage) / 100;

                _logger.LogInformation("Loss share calculated: {LossShare} for InvestmentId: {InvestmentId}", 
                    lossShare, investmentId);

                return GenericResponse<decimal>.SuccessResponse(lossShare, 200, "Loss share calculated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating loss share for InvestmentId: {InvestmentId}", investmentId);
                return GenericResponse<decimal>.FailResponse($"Error calculating loss share: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<decimal>> CalculateEarlyWithdrawalPenaltyAsync(string investmentId)
        {
            _logger.LogInformation("Calculating early withdrawal penalty for InvestmentId: {InvestmentId}", investmentId);

            try
            {
                var investment = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
                if (investment == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", investmentId);
                    return GenericResponse<decimal>.FailResponse("Investment not found", 404);
                }

                var poolTenor = await _tenorRepository.GetPoolTenorByPoolIdAsync(investment.PoolId);
                if (poolTenor == null)
                {
                    _logger.LogWarning("Pool tenor not found for PoolId: {PoolId}", investment.PoolId);
                    return GenericResponse<decimal>.FailResponse("Pool tenor not found", 404);
                }

                // Calculate penalty based on remaining time and investment amount
                var remainingMonths = (poolTenor.ExpectedEndDate - DateTime.UtcNow).Days / 30.0;
                var totalMonths = poolTenor.DurationInMonths;
                var penaltyPercentage = (remainingMonths / totalMonths) * 0.15; // 15% max penalty
                var penalty = investment.InvestmentAmount * (decimal)penaltyPercentage;

                _logger.LogInformation("Early withdrawal penalty calculated: {Penalty} for InvestmentId: {InvestmentId}", 
                    penalty, investmentId);

                return GenericResponse<decimal>.SuccessResponse(penalty, 200, "Early withdrawal penalty calculated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating early withdrawal penalty for InvestmentId: {InvestmentId}", investmentId);
                return GenericResponse<decimal>.FailResponse($"Error calculating penalty: {ex.Message}", 500);
            }
        }

        // Additional methods for getting investments and pool tenor
        public async Task<GenericResponse<LenderInvestmentDto>> GetInvestmentByIdAsync(string investmentId)
        {
            _logger.LogInformation("Fetching investment by Id: {InvestmentId}", investmentId);
            try
            {
                var investment = await _investmentRepository.GetInvestmentByIdAsync(investmentId);
                if (investment == null)
                {
                    _logger.LogWarning("Investment not found: {InvestmentId}", investmentId);
                    return GenericResponse<LenderInvestmentDto>.FailResponse("Investment not found", 404);
                }
                return GenericResponse<LenderInvestmentDto>.SuccessResponse(MapToDto(investment), 200, "Investment retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching investment: {InvestmentId}", investmentId);
                return GenericResponse<LenderInvestmentDto>.FailResponse($"Error fetching investment: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<IEnumerable<LenderInvestmentDto>>> GetInvestmentsByPoolIdAsync(string poolId)
        {
            _logger.LogInformation("Fetching investments for PoolId: {PoolId}", poolId);
            try
            {
                var investments = await _investmentRepository.GetInvestmentsByPoolIdAsync(poolId);
                var dtos = investments.Select(MapToDto);
                return GenericResponse<IEnumerable<LenderInvestmentDto>>.SuccessResponse(dtos, 200, "Investments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching investments for PoolId: {PoolId}", poolId);
                return GenericResponse<IEnumerable<LenderInvestmentDto>>.FailResponse($"Error fetching investments: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<IEnumerable<LenderInvestmentDto>>> GetInvestmentsByLenderIdAsync(string lenderId)
        {
            _logger.LogInformation("Fetching investments for LenderId: {LenderId}", lenderId);
            try
            {
                var investments = await _investmentRepository.GetInvestmentsByLenderIdAsync(lenderId);
                var dtos = investments.Select(MapToDto);
                return GenericResponse<IEnumerable<LenderInvestmentDto>>.SuccessResponse(dtos, 200, "Investments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching investments for LenderId: {LenderId}", lenderId);
                return GenericResponse<IEnumerable<LenderInvestmentDto>>.FailResponse($"Error fetching investments: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<PoolTenorDto>> GetPoolTenorAsync(string poolId)
        {
            _logger.LogInformation("Fetching pool tenor for PoolId: {PoolId}", poolId);
            try
            {
                var tenor = await _tenorRepository.GetPoolTenorByPoolIdAsync(poolId);
                if (tenor == null)
                {
                    _logger.LogWarning("Pool tenor not found: {PoolId}", poolId);
                    return GenericResponse<PoolTenorDto>.FailResponse("Pool tenor not found", 404);
                }
                return GenericResponse<PoolTenorDto>.SuccessResponse(MapToTenorDto(tenor), 200, "Pool tenor retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pool tenor: {PoolId}", poolId);
                return GenericResponse<PoolTenorDto>.FailResponse($"Error fetching pool tenor: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<bool>> UpdatePoolTenorStatusAsync(string poolId, string status)
        {
            _logger.LogInformation("Updating pool tenor status for PoolId: {PoolId} to {Status}", poolId, status);
            try
            {
                var result = await _tenorRepository.UpdatePoolTenorStatusAsync(poolId, status);
                return GenericResponse<bool>.SuccessResponse(result, 200, "Pool tenor status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pool tenor status: {PoolId}", poolId);
                return GenericResponse<bool>.FailResponse($"Error updating pool tenor status: {ex.Message}", 500);
            }
        }

        private LenderInvestmentDto MapToDto(LenderInvestment investment)
        {
            return new LenderInvestmentDto
            {
                Id = investment.Id,
                LenderId = investment.LenderId,
                PoolId = investment.PoolId,
                InvestmentAmount = investment.InvestmentAmount,
                InvestmentPercentage = investment.InvestmentPercentage,
                InvestmentDate = investment.InvestmentDate,
                WithdrawalDate = investment.WithdrawalDate,
                WithdrawalAmount = investment.WithdrawalAmount,
                ProfitEarned = investment.ProfitEarned,
                LossIncurred = investment.LossIncurred,
                Status = investment.Status,
                WithdrawalReason = investment.WithdrawalReason,
                IsEarlyWithdrawal = investment.IsEarlyWithdrawal,
                EarlyWithdrawalPenalty = investment.EarlyWithdrawalPenalty
            };
        }

        private PoolTenorDto MapToTenorDto(PoolTenor tenor)
        {
            return new PoolTenorDto
            {
                Id = tenor.Id,
                PoolId = tenor.PoolId,
                DurationInMonths = tenor.DurationInMonths,
                StartDate = tenor.StartDate,
                ExpectedEndDate = tenor.ExpectedEndDate,
                ActualEndDate = tenor.ActualEndDate,
                TotalPoolAmount = tenor.TotalPoolAmount,
                TotalLentAmount = tenor.TotalLentAmount,
                TotalProfit = tenor.TotalProfit,
                TotalLoss = tenor.TotalLoss,
                Status = tenor.Status
            };
        }
    }
} 