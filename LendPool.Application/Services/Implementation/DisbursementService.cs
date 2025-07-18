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
    public class DisbursementService : IDisbursementService
    {
        private readonly IDisbursementRepository _disbursementRepository;
        private readonly ILogger<DisbursementService> _logger;
        public DisbursementService(IDisbursementRepository disbursementRepository, ILogger<DisbursementService> logger)
        {
            _disbursementRepository = disbursementRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<DisbursementDto>> CreateDisbursementAsync(CreateDisbursementDto dto)
        {
            _logger.LogInformation("Creating disbursement for LoanId: {LoanId}, BorrowerId: {BorrowerId}, LenderId: {LenderId}, Amount: {Amount}", dto.LoanId, dto.BorrowerId, dto.LenderId, dto.Amount);
            try
            {
                var disbursement = new Disbursement
                {
                    LoanId = dto.LoanId,
                    BorrowerId = dto.BorrowerId,
                    LenderId = dto.LenderId,
                    Amount = dto.Amount,
                    DisbursementDate = DateTime.UtcNow,
                    Description = dto.Description,
                    Status = DisbursementStatus.Completed.ToString(),
                    FundUsages = new List<FundUsage>()
                };
                await _disbursementRepository.AddAsync(disbursement);
                await _disbursementRepository.SaveAsync();
                _logger.LogInformation("Disbursement created: {DisbursementId} for Loan {LoanId}", disbursement.Id, disbursement.LoanId);
                var newDisbursement = new DisbursementDto
                {
                    Id = disbursement.Id,
                    LoanId = disbursement.LoanId,
                    BorrowerId = disbursement.BorrowerId,
                    LenderId = disbursement.LenderId,
                    Amount = disbursement.Amount,
                    DisbursementDate = disbursement.DisbursementDate,
                    Description = disbursement.Description,
                    Status = disbursement.Status,
                    FundUsages = new List<FundUsageDto>()
                };
                return GenericResponse<DisbursementDto>.SuccessResponse(newDisbursement, 201, "Disbursement created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating disbursement for LoanId: {LoanId}", dto.LoanId);
                return GenericResponse<DisbursementDto>.FailResponse($"Error creating disbursement: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<IEnumerable<DisbursementDto>>> GetDisbursementsByLoanIdAsync(string loanId)
        {
            _logger.LogInformation("Fetching disbursements for LoanId: {LoanId}", loanId);
            try
            {
                var disbursements = await _disbursementRepository.GetDisbursementsByLoanIdAsync(loanId);
                _logger.LogInformation("Fetched {Count} disbursements for Loan {LoanId}", disbursements.Count(), loanId);
                var disbursment = disbursements.Select(d => new DisbursementDto
                {
                    Id = d.Id,
                    LoanId = d.LoanId,
                    BorrowerId = d.BorrowerId,
                    LenderId = d.LenderId,
                    Amount = d.Amount,
                    DisbursementDate = d.DisbursementDate,
                    Description = d.Description,
                    Status = d.Status,
                    FundUsages = d.FundUsages?.Select(fu => new FundUsageDto
                    {
                        Id = fu.Id,
                        DisbursementId = fu.DisbursementId,
                        BorrowerId = fu.BorrowerId,
                        AmountUsed = fu.AmountUsed,
                        Category = fu.Category,
                        Description = fu.Description,
                        Timestamp = fu.Timestamp,
                        AttachmentUrl = fu.AttachmentUrl
                    }).ToList() ?? new List<FundUsageDto>()
                });
                return GenericResponse<IEnumerable<DisbursementDto>>.SuccessResponse(disbursment, 200, "Disbursements retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching disbursements for LoanId: {LoanId}", loanId);
                return GenericResponse<IEnumerable<DisbursementDto>>.FailResponse($"Error fetching disbursements: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<DisbursementDto>> GetDisbursementByIdAsync(string id)
        {
            _logger.LogInformation("Fetching disbursement by Id: {DisbursementId}", id);
            try
            {
                var d = await _disbursementRepository.GetDisbursementWithFundUsagesAsync(id);
                if (d == null)
                {
                    _logger.LogWarning("Disbursement not found: {DisbursementId}", id);
                    return GenericResponse<DisbursementDto>.FailResponse("Disbursement not found", 404);
                }
                _logger.LogInformation("Fetched disbursement {DisbursementId}", id);
                var disbursement = new DisbursementDto
                {
                    Id = d.Id,
                    LoanId = d.LoanId,
                    BorrowerId = d.BorrowerId,
                    LenderId = d.LenderId,
                    Amount = d.Amount,
                    DisbursementDate = d.DisbursementDate,
                    Description = d.Description,
                    Status = d.Status,
                    FundUsages = d.FundUsages?.Select(fu => new FundUsageDto
                    {
                        Id = fu.Id,
                        DisbursementId = fu.DisbursementId,
                        BorrowerId = fu.BorrowerId,
                        AmountUsed = fu.AmountUsed,
                        Category = fu.Category,
                        Description = fu.Description,
                        Timestamp = fu.Timestamp,
                        AttachmentUrl = fu.AttachmentUrl
                    }).ToList() ?? new List<FundUsageDto>()
                };
                return GenericResponse<DisbursementDto>.SuccessResponse(disbursement, 200, "Disbursement retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching disbursement by Id: {DisbursementId}", id);
                return GenericResponse<DisbursementDto>.FailResponse($"Error fetching disbursement: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<FundUsageDto>> LogFundUsageAsync(CreateFundUsageDto dto)
        {
            _logger.LogInformation("Logging fund usage for DisbursementId: {DisbursementId}, BorrowerId: {BorrowerId}, AmountUsed: {AmountUsed}", dto.DisbursementId, dto.BorrowerId, dto.AmountUsed);
            try
            {
                var fundUsage = new FundUsage
                {
                    DisbursementId = dto.DisbursementId,
                    BorrowerId = dto.BorrowerId,
                    AmountUsed = dto.AmountUsed,
                    Category = dto.Category,
                    Description = dto.Description,
                    Timestamp = DateTime.UtcNow,
                    AttachmentUrl = dto.AttachmentUrl
                };
                var savedFundUsage = await _disbursementRepository.AddFundUsageAsync(fundUsage);
                _logger.LogInformation("Fund usage logged: {FundUsageId} for Disbursement {DisbursementId}", savedFundUsage.Id, savedFundUsage.DisbursementId);
                var fundUsageDto = new FundUsageDto
                {
                    Id = savedFundUsage.Id,
                    DisbursementId = savedFundUsage.DisbursementId,
                    BorrowerId = savedFundUsage.BorrowerId,
                    AmountUsed = savedFundUsage.AmountUsed,
                    Category = savedFundUsage.Category,
                    Description = savedFundUsage.Description,
                    Timestamp = savedFundUsage.Timestamp,
                    AttachmentUrl = savedFundUsage.AttachmentUrl
                };
                return GenericResponse<FundUsageDto>.SuccessResponse(fundUsageDto, 201, "Fund usage logged successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging fund usage for DisbursementId: {DisbursementId}", dto.DisbursementId);
                return GenericResponse<FundUsageDto>.FailResponse($"Error logging fund usage: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<IEnumerable<FundUsageDto>>> GetFundUsagesByDisbursementIdAsync(string disbursementId)
        {
            _logger.LogInformation("Fetching fund usages for DisbursementId: {DisbursementId}", disbursementId);
            try
            {
                var usages = await _disbursementRepository.GetFundUsagesByDisbursementIdAsync(disbursementId);
                _logger.LogInformation("Fetched {Count} fund usages for Disbursement {DisbursementId}", usages.Count(), disbursementId);
                var usage = usages.Select(fu => new FundUsageDto
                {
                    Id = fu.Id,
                    DisbursementId = fu.DisbursementId,
                    BorrowerId = fu.BorrowerId,
                    AmountUsed = fu.AmountUsed,
                    Category = fu.Category,
                    Description = fu.Description,
                    Timestamp = fu.Timestamp,
                    AttachmentUrl = fu.AttachmentUrl
                });
                return GenericResponse<IEnumerable<FundUsageDto>>.SuccessResponse(usage, 200, "Fund usages retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fund usages for DisbursementId: {DisbursementId}", disbursementId);
                return GenericResponse<IEnumerable<FundUsageDto>>.FailResponse($"Error fetching fund usages: {ex.Message}", 500);
            }
        }
    }
} 