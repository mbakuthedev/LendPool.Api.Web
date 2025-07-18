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
    public class ReconciliationService : IReconciliationService
    {
        private readonly IReconciliationRepository _reconciliationRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IDisbursementRepository _disbursementRepository;
        private readonly ILogger<ReconciliationService> _logger;

        public ReconciliationService(
            IReconciliationRepository reconciliationRepository,
            ILoanRepository loanRepository,
            IDisbursementRepository disbursementRepository,
            ILogger<ReconciliationService> logger)
        {
            _reconciliationRepository = reconciliationRepository;
            _loanRepository = loanRepository;
            _disbursementRepository = disbursementRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<ReconciliationResponseDto>> CreateReconciliationRequestAsync(string lenderId, CreateReconciliationRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Creating reconciliation request for loan {LoanId} by lender {LenderId}", dto.LoanId, lenderId);

                // Verify loan exists and lender has access
                var loan = await _loanRepository.GetLoanByIdAsync(dto.LoanId);
                if (loan.Data == null)
                {
                    _logger.LogWarning("Loan {LoanId} not found for reconciliation request", dto.LoanId);
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Loan not found", 404);
                }

                // Check if reconciliation already exists
                var existingReconciliation = await _reconciliationRepository.GetReconciliationByLoanAsync(dto.LoanId);
                if (existingReconciliation.Data != null)
                {
                    _logger.LogWarning("Reconciliation already exists for loan {LoanId}", dto.LoanId);
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Reconciliation already exists for this loan", 400);
                }

                // Get disbursements for the loan
                var disbursements = await _disbursementRepository.GetDisbursementsByLoanAsync(dto.LoanId);
                var totalDisbursed = disbursements.Data?.Sum(d => d.Amount) ?? 0;

                var reconciliation = new LoanReconciliation
                {
                    LoanId = dto.LoanId,
                    LenderId = lenderId,
                    BorrowerId = dto.BorrowerId,
                    ReconciliationStatus = ReconciliationStatus.Pending.ToString(),
                    RequestedBy = lenderId,
                    Comments = dto.Comments,
                    TotalDisbursedAmount = totalDisbursed,
                    TotalVerifiedAmount = 0,
                    DiscrepancyAmount = 0,
                    RequestedDate = DateTime.UtcNow
                };

                var result = await _reconciliationRepository.CreateReconciliationAsync(reconciliation);
                if (!result.Success)
                {
                    _logger.LogError("Failed to create reconciliation for loan {LoanId}", dto.LoanId);
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Failed to create reconciliation", 500);
                }

                // Create reconciliation items for each fund usage
                await CreateReconciliationItemsAsync(reconciliation.Id, dto.LoanId);

                var responseDto = MapToReconciliationResponseDto(result.Data);
                _logger.LogInformation("Successfully created reconciliation {ReconciliationId} for loan {LoanId}", reconciliation.Id, dto.LoanId);

                return GenericResponse<ReconciliationResponseDto>.SuccessResponse(responseDto, 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reconciliation request for loan {LoanId}", dto.LoanId);
                return GenericResponse<ReconciliationResponseDto>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationResponseDto>> GetReconciliationByIdAsync(string reconciliationId)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByIdAsync(reconciliationId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Reconciliation not found", 404);
                }

                var responseDto = MapToReconciliationResponseDto(reconciliation.Data);
                return GenericResponse<ReconciliationResponseDto>.SuccessResponse(responseDto, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliation {ReconciliationId}", reconciliationId);
                return GenericResponse<ReconciliationResponseDto>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByLoanAsync(string loanId)
        {
            try
            {
                var reconciliations = await _reconciliationRepository.GetReconciliationsByLoanAsync(loanId);
                var responseDtos = reconciliations.Data?.Select(MapToReconciliationResponseDto).ToList() ?? new List<ReconciliationResponseDto>();
                return GenericResponse<List<ReconciliationResponseDto>>.SuccessResponse(responseDtos, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for loan {LoanId}", loanId);
                return GenericResponse<List<ReconciliationResponseDto>>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByLenderAsync(string lenderId)
        {
            try
            {
                var reconciliations = await _reconciliationRepository.GetReconciliationsByLenderAsync(lenderId);
                var responseDtos = reconciliations.Data?.Select(MapToReconciliationResponseDto).ToList() ?? new List<ReconciliationResponseDto>();
                return GenericResponse<List<ReconciliationResponseDto>>.SuccessResponse(responseDtos, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for lender {LenderId}", lenderId);
                return GenericResponse<List<ReconciliationResponseDto>>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByBorrowerAsync(string borrowerId)
        {
            try
            {
                var reconciliations = await _reconciliationRepository.GetReconciliationsByBorrowerAsync(borrowerId);
                var responseDtos = reconciliations.Data?.Select(MapToReconciliationResponseDto).ToList() ?? new List<ReconciliationResponseDto>();
                return GenericResponse<List<ReconciliationResponseDto>>.SuccessResponse(responseDtos, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reconciliations for borrower {BorrowerId}", borrowerId);
                return GenericResponse<List<ReconciliationResponseDto>>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationResponseDto>> UpdateReconciliationStatusAsync(string reconciliationId, string status, string comments = null)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByIdAsync(reconciliationId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Reconciliation not found", 404);
                }

                reconciliation.Data.ReconciliationStatus = status;
                if (status == ReconciliationStatus.Completed.ToString())
                {
                    reconciliation.Data.CompletedDate = DateTime.UtcNow;
                }

                if (!string.IsNullOrEmpty(comments))
                {
                    reconciliation.Data.Comments = comments;
                }

                var result = await _reconciliationRepository.UpdateReconciliationAsync(reconciliation.Data);
                if (!result.Success)
                {
                    return GenericResponse<ReconciliationResponseDto>.FailResponse("Failed to update reconciliation", 500);
                }

                var responseDto = MapToReconciliationResponseDto(result.Data);
                return GenericResponse<ReconciliationResponseDto>.SuccessResponse(responseDto, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reconciliation status for {ReconciliationId}", reconciliationId);
                return GenericResponse<ReconciliationResponseDto>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationItemDto>> UpdateReconciliationItemAsync(string itemId, UpdateReconciliationItemDto dto)
        {
            try
            {
                var item = await _reconciliationRepository.GetReconciliationItemByIdAsync(itemId);
                if (item.Data == null)
                {
                    return GenericResponse<ReconciliationItemDto>.FailResponse("Reconciliation item not found", 404);
                }

                item.Data.VerificationStatus = dto.VerificationStatus;
                item.Data.LenderComments = dto.LenderComments;
                item.Data.IsVerified = dto.IsVerified;
                item.Data.IsCompliant = dto.IsCompliant;
                item.Data.ComplianceNotes = dto.ComplianceNotes;

                if (dto.IsVerified)
                {
                    item.Data.VerifiedDate = DateTime.UtcNow;
                }

                var result = await _reconciliationRepository.UpdateReconciliationItemAsync(item.Data);
                if (!result.Success)
                {
                    return GenericResponse<ReconciliationItemDto>.FailResponse("Failed to update reconciliation item", 500);
                }

                var responseDto = MapToReconciliationItemDto(result.Data);
                return GenericResponse<ReconciliationItemDto>.SuccessResponse(responseDto, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reconciliation item {ItemId}", itemId);
                return GenericResponse<ReconciliationItemDto>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<bool>> VerifyFundUsageAsync(string fundUsageId, FundUsageVerificationDto dto)
        {
            try
            {
                var fundUsage = await _disbursementRepository.GetFundUsageByIdAsync(fundUsageId);
                if (fundUsage.Data == null)
                {
                    return GenericResponse<bool>.FailResponse("Fund usage not found", 404);
                }

                // Update fund usage verification status
                fundUsage.Data.VerificationStatus = dto.VerificationStatus;
                fundUsage.Data.ReceiptUrl = dto.ReceiptUrl;
                fundUsage.Data.IsCompliant = dto.IsCompliant;
                fundUsage.Data.ComplianceNotes = dto.ComplianceNotes;

                var result = await _disbursementRepository.UpdateFundUsageAsync(fundUsage.Data);
                if (!result.Success)
                {
                    return GenericResponse<bool>.FailResponse("Failed to update fund usage", 500);
                }

                // Update corresponding reconciliation item
                var reconciliationItem = await _reconciliationRepository.GetReconciliationItemByFundUsageAsync(fundUsageId);
                if (reconciliationItem.Data != null)
                {
                    reconciliationItem.Data.VerificationStatus = dto.VerificationStatus;
                    reconciliationItem.Data.IsVerified = dto.VerificationStatus == VerificationStatus.Verified.ToString();
                    reconciliationItem.Data.IsCompliant = dto.IsCompliant;
                    reconciliationItem.Data.ComplianceNotes = dto.ComplianceNotes;
                    reconciliationItem.Data.ReceiptUrl = dto.ReceiptUrl;

                    if (reconciliationItem.Data.IsVerified ?? false)
                    {
                        reconciliationItem.Data.VerifiedDate = DateTime.UtcNow;
                    }

                    await _reconciliationRepository.UpdateReconciliationItemAsync(reconciliationItem.Data);
                }

                return GenericResponse<bool>.SuccessResponse(true, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying fund usage {FundUsageId}", fundUsageId);
                return GenericResponse<bool>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<ReconciliationSummaryDto>> GetReconciliationSummaryAsync(string loanId)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByLoanAsync(loanId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<ReconciliationSummaryDto>.FailResponse("Reconciliation not found", 404);
                }

                var items = await _reconciliationRepository.GetReconciliationItemsAsync(reconciliation.Data.Id);
                var summary = new ReconciliationSummaryDto
                {
                    LoanId = loanId,
                    BorrowerName = reconciliation.Data.Borrower?.FullName ?? "Unknown",
                    TotalDisbursed = reconciliation.Data.TotalDisbursedAmount,
                    TotalVerified = reconciliation.Data.TotalVerifiedAmount,
                    DiscrepancyAmount = reconciliation.Data.DiscrepancyAmount,
                    ReconciliationStatus = reconciliation.Data.ReconciliationStatus,
                    IsCompliant = reconciliation.Data.IsCompliant ?? false,
                    TotalItems = items.Data?.Count ?? 0,
                    VerifiedItems = items.Data?.Count(i => (i.IsVerified ?? false)) ?? 0,
                    PendingItems = items.Data?.Count(i => i.VerificationStatus == VerificationStatus.Pending.ToString()) ?? 0,
                    DisputedItems = items.Data?.Count(i => i.VerificationStatus == VerificationStatus.Disputed.ToString()) ?? 0
                };

                return GenericResponse<ReconciliationSummaryDto>.SuccessResponse(summary, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reconciliation summary for loan {LoanId}", loanId);
                return GenericResponse<ReconciliationSummaryDto>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<bool>> CompleteReconciliationAsync(string reconciliationId, string lenderId)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByIdAsync(reconciliationId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<bool>.FailResponse("Reconciliation not found", 404);
                }

                // Calculate final amounts
                var discrepancy = await CalculateDiscrepancyAsync(reconciliationId);
                reconciliation.Data.DiscrepancyAmount = discrepancy.Data;
                reconciliation.Data.TotalVerifiedAmount = reconciliation.Data.TotalDisbursedAmount - discrepancy.Data;
                reconciliation.Data.ReconciliationStatus = ReconciliationStatus.Completed.ToString();
                reconciliation.Data.CompletedDate = DateTime.UtcNow;
                reconciliation.Data.IsCompliant = discrepancy.Data == 0;

                var result = await _reconciliationRepository.UpdateReconciliationAsync(reconciliation.Data);
                return GenericResponse<bool>.SuccessResponse(result.Success, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing reconciliation {ReconciliationId}", reconciliationId);
                return GenericResponse<bool>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<List<ReconciliationSummaryDto>>> GetReconciliationSummariesByPoolAsync(string poolId)
        {
            try
            {
                var loans = await _loanRepository.GetLoansByPoolAsync(poolId);
                var summaries = new List<ReconciliationSummaryDto>();

                foreach (var loan in loans.Data ?? new List<Loan>())
                {
                    var summary = await GetReconciliationSummaryAsync(loan.Id);
                    if (summary.Success)
                    {
                        summaries.Add(summary.Data);
                    }
                }

                return GenericResponse<List<ReconciliationSummaryDto>>.SuccessResponse(summaries, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reconciliation summaries for pool {PoolId}", poolId);
                return GenericResponse<List<ReconciliationSummaryDto>>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<decimal>> CalculateDiscrepancyAsync(string reconciliationId)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByIdAsync(reconciliationId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<decimal>.FailResponse("Reconciliation not found", 404);
                }

                var items = await _reconciliationRepository.GetReconciliationItemsAsync(reconciliationId);
                var verifiedAmount = items.Data?.Where(i => (i.IsVerified ?? false) && (i.IsCompliant ?? false)).Sum(i => i.Amount) ?? 0;
                var discrepancy = reconciliation.Data.TotalDisbursedAmount - verifiedAmount;

                return GenericResponse<decimal>.SuccessResponse(discrepancy, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating discrepancy for reconciliation {ReconciliationId}", reconciliationId);
                return GenericResponse<decimal>.FailResponse("Internal server error", 500);
            }
        }

        public async Task<GenericResponse<bool>> MarkAsCompliantAsync(string reconciliationId, bool isCompliant, string notes = null)
        {
            try
            {
                var reconciliation = await _reconciliationRepository.GetReconciliationByIdAsync(reconciliationId);
                if (reconciliation.Data == null)
                {
                    return GenericResponse<bool>.FailResponse("Reconciliation not found", 404);
                }

                reconciliation.Data.IsCompliant = isCompliant;
                reconciliation.Data.ComplianceNotes = notes;

                var result = await _reconciliationRepository.UpdateReconciliationAsync(reconciliation.Data);
                return GenericResponse<bool>.SuccessResponse(result.Success, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking reconciliation as compliant {ReconciliationId}", reconciliationId);
                return GenericResponse<bool>.FailResponse("Internal server error", 500);
            }
        }

        private async Task CreateReconciliationItemsAsync(string reconciliationId, string loanId)
        {
            try
            {
                var disbursements = await _disbursementRepository.GetDisbursementsByLoanAsync(loanId);
                var fundUsages = await _disbursementRepository.GetFundUsagesByLoanAsync(loanId);

                foreach (var fundUsage in fundUsages.Data ?? new List<FundUsage>())
                {
                    var item = new ReconciliationItem
                    {
                        ReconciliationId = reconciliationId,
                        FundUsageId = fundUsage.Id,
                        DisbursementId = fundUsage.DisbursementId,
                        Amount = fundUsage.AmountUsed,
                        Category = fundUsage.Category,
                        Description = fundUsage.Description,
                        VerificationStatus = VerificationStatus.Pending.ToString(),
                        AttachmentUrl = fundUsage.AttachmentUrl,
                        Timestamp = fundUsage.Timestamp
                    };

                    await _reconciliationRepository.CreateReconciliationItemAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reconciliation items for reconciliation {ReconciliationId}", reconciliationId);
                throw;
            }
        }

        private ReconciliationResponseDto MapToReconciliationResponseDto(LoanReconciliation reconciliation)
        {
            return new ReconciliationResponseDto
            {
                Id = reconciliation.Id,
                LoanId = reconciliation.LoanId,
                BorrowerId = reconciliation.BorrowerId,
                LenderId = reconciliation.LenderId,
                ReconciliationStatus = reconciliation.ReconciliationStatus,
                RequestedDate = reconciliation.RequestedDate,
                CompletedDate = reconciliation.CompletedDate,
                Comments = reconciliation.Comments,
                TotalDisbursedAmount = reconciliation.TotalDisbursedAmount,
                TotalVerifiedAmount = reconciliation.TotalVerifiedAmount,
                DiscrepancyAmount = reconciliation.DiscrepancyAmount,
                DiscrepancyReason = reconciliation.DiscrepancyReason,
                IsCompliant = reconciliation.IsCompliant ?? false,
                ComplianceNotes = reconciliation.ComplianceNotes,
                Items = reconciliation.ReconciliationItems?.Select(MapToReconciliationItemDto).ToList() ?? new List<ReconciliationItemDto>()
            };
        }

        private ReconciliationItemDto MapToReconciliationItemDto(ReconciliationItem item)
        {
            return new ReconciliationItemDto
            {
                Id = item.Id,
                FundUsageId = item.FundUsageId,
                DisbursementId = item.DisbursementId,
                Amount = item.Amount,
                Category = item.Category,
                Description = item.Description,
                VerificationStatus = item.VerificationStatus,
                LenderComments = item.LenderComments,
                BorrowerComments = item.BorrowerComments,
                IsVerified = item.IsVerified ?? false,
                VerifiedDate = item.VerifiedDate,
                VerifiedBy = item.VerifiedBy,
                AttachmentUrl = item.AttachmentUrl,
                ReceiptUrl = item.ReceiptUrl,
                IsCompliant = item.IsCompliant ?? false,
                ComplianceNotes = item.ComplianceNotes
            };
        }
    }
} 