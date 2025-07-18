using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Lender")]
    public class ReconciliationController : BaseController
    {
        private readonly IReconciliationService _reconciliationService;
        private readonly ILogger<ReconciliationController> _logger;

        public ReconciliationController(IReconciliationService reconciliationService, ILogger<ReconciliationController> logger)
        {
            _reconciliationService = reconciliationService;
            _logger = logger;
        }

        [HttpPost("reconciliation/request")]
        public async Task<IActionResult> CreateReconciliationRequest([FromBody] CreateReconciliationRequestDto dto)
        {
            var lenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Lender {LenderId} requesting reconciliation for loan {LoanId}", lenderId, dto.LoanId);

            var result = await _reconciliationService.CreateReconciliationRequestAsync(lenderId, dto);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create reconciliation request for loan {LoanId}", dto.LoanId);
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpGet("reconciliation/{reconciliationId}")]
        public async Task<IActionResult> GetReconciliation(string reconciliationId)
        {
            var result = await _reconciliationService.GetReconciliationByIdAsync(reconciliationId);
            if (!result.Success)
            {
                return NotFoundResponse(result.Message);
            }

            return Success(result);
        }

        [HttpGet("reconciliation/loan/{loanId}")]
        public async Task<IActionResult> GetReconciliationsByLoan(string loanId)
        {
            var result = await _reconciliationService.GetReconciliationsByLoanAsync(loanId);
            return Success(result);
        }

        [HttpGet("reconciliation/lender/my-reconciliations")]
        public async Task<IActionResult> GetMyReconciliations()
        {
            var lenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reconciliationService.GetReconciliationsByLenderAsync(lenderId);
            return Success(result);
        }

        [HttpPut("reconciliation/{reconciliationId}/status")]
        public async Task<IActionResult> UpdateReconciliationStatus(string reconciliationId, [FromQuery] string status, [FromQuery] string comments = null)
        {
            var result = await _reconciliationService.UpdateReconciliationStatusAsync(reconciliationId, status, comments);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPut("reconciliation/item/{itemId}")]
        public async Task<IActionResult> UpdateReconciliationItem(string itemId, [FromBody] UpdateReconciliationItemDto dto)
        {
            var result = await _reconciliationService.UpdateReconciliationItemAsync(itemId, dto);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPost("reconciliation/verify-fund-usage/{fundUsageId}")]
        public async Task<IActionResult> VerifyFundUsage(string fundUsageId, [FromBody] FundUsageVerificationDto dto)
        {
            var result = await _reconciliationService.VerifyFundUsageAsync(fundUsageId, dto);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpGet("reconciliation/summary/{loanId}")]
        public async Task<IActionResult> GetReconciliationSummary(string loanId)
        {
            var result = await _reconciliationService.GetReconciliationSummaryAsync(loanId);
            if (!result.Success)
            {
                return NotFoundResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPost("reconciliation/{reconciliationId}/complete")]
        public async Task<IActionResult> CompleteReconciliation(string reconciliationId)
        {
            var lenderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reconciliationService.CompleteReconciliationAsync(reconciliationId, lenderId);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpGet("reconciliation/pool/{poolId}/summaries")]
        public async Task<IActionResult> GetReconciliationSummariesByPool(string poolId)
        {
            var result = await _reconciliationService.GetReconciliationSummariesByPoolAsync(poolId);
            return Success(result);
        }

        [HttpGet("reconciliation/{reconciliationId}/discrepancy")]
        public async Task<IActionResult> CalculateDiscrepancy(string reconciliationId)
        {
            var result = await _reconciliationService.CalculateDiscrepancyAsync(reconciliationId);
            if (!result.Success)
            {
                return NotFoundResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPut("reconciliation/{reconciliationId}/compliance")]
        public async Task<IActionResult> MarkAsCompliant(string reconciliationId, [FromQuery] bool isCompliant, [FromQuery] string notes = null)
        {
            var result = await _reconciliationService.MarkAsCompliantAsync(reconciliationId, isCompliant, notes);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }
    }
} 