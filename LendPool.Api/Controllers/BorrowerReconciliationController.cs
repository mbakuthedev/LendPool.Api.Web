using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Borrower")]
    public class BorrowerReconciliationController : BaseController
    {
        private readonly IReconciliationService _reconciliationService;
        private readonly ILogger<BorrowerReconciliationController> _logger;

        public BorrowerReconciliationController(IReconciliationService reconciliationService, ILogger<BorrowerReconciliationController> logger)
        {
            _reconciliationService = reconciliationService;
            _logger = logger;
        }

        [HttpGet("borrower/reconciliation/my-reconciliations")]
        public async Task<IActionResult> GetMyReconciliations()
        {
            var borrowerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reconciliationService.GetReconciliationsByBorrowerAsync(borrowerId);
            return Success(result);
        }

        [HttpGet("borrower/reconciliation/{reconciliationId}")]
        public async Task<IActionResult> GetReconciliation(string reconciliationId)
        {
            var result = await _reconciliationService.GetReconciliationByIdAsync(reconciliationId);
            if (!result.Success)
            {
                return NotFoundResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPut("borrower/reconciliation/item/{itemId}/respond")]
        public async Task<IActionResult> RespondToReconciliationItem(string itemId, [FromBody] UpdateReconciliationItemDto dto)
        {
            var borrowerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Borrower {BorrowerId} responding to reconciliation item {ItemId}", borrowerId, itemId);

            var result = await _reconciliationService.UpdateReconciliationItemAsync(itemId, dto);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpPost("borrower/reconciliation/verify-fund-usage/{fundUsageId}")]
        public async Task<IActionResult> VerifyFundUsage(string fundUsageId, [FromBody] FundUsageVerificationDto dto)
        {
            var borrowerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Borrower {BorrowerId} verifying fund usage {FundUsageId}", borrowerId, fundUsageId);

            var result = await _reconciliationService.VerifyFundUsageAsync(fundUsageId, dto);
            if (!result.Success)
            {
                return BadRequestResponse(result.Message);
            }

            return Success(result);
        }

        [HttpGet("borrower/reconciliation/summary/{loanId}")]
        public async Task<IActionResult> GetReconciliationSummary(string loanId)
        {
            var result = await _reconciliationService.GetReconciliationSummaryAsync(loanId);
            if (!result.Success)
            {
                return NotFoundResponse(result.Message);
            }

            return Success(result);
        }
    }
} 