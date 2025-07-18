using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IReconciliationService
    {
        Task<GenericResponse<ReconciliationResponseDto>> CreateReconciliationRequestAsync(string lenderId, CreateReconciliationRequestDto dto);
        Task<GenericResponse<ReconciliationResponseDto>> GetReconciliationByIdAsync(string reconciliationId);
        Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByLoanAsync(string loanId);
        Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByLenderAsync(string lenderId);
        Task<GenericResponse<List<ReconciliationResponseDto>>> GetReconciliationsByBorrowerAsync(string borrowerId);
        Task<GenericResponse<ReconciliationResponseDto>> UpdateReconciliationStatusAsync(string reconciliationId, string status, string comments = null);
        Task<GenericResponse<ReconciliationItemDto>> UpdateReconciliationItemAsync(string itemId, UpdateReconciliationItemDto dto);
        Task<GenericResponse<bool>> VerifyFundUsageAsync(string fundUsageId, FundUsageVerificationDto dto);
        Task<GenericResponse<ReconciliationSummaryDto>> GetReconciliationSummaryAsync(string loanId);
        Task<GenericResponse<bool>> CompleteReconciliationAsync(string reconciliationId, string lenderId);
        Task<GenericResponse<List<ReconciliationSummaryDto>>> GetReconciliationSummariesByPoolAsync(string poolId);
        Task<GenericResponse<decimal>> CalculateDiscrepancyAsync(string reconciliationId);
        Task<GenericResponse<bool>> MarkAsCompliantAsync(string reconciliationId, bool isCompliant, string notes = null);
    }
} 