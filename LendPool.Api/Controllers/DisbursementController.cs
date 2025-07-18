using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LendPool.Api.Controllers
{
 
    public class DisbursementController : BaseController
    {
        private readonly IDisbursementService _disbursementService;
        private readonly ILogger<DisbursementController> _logger;

        public DisbursementController(IDisbursementService disbursementService, ILogger<DisbursementController> logger)
        {
            _disbursementService = disbursementService;
            _logger = logger;
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpPost("disbursement/create-disbursement")]
        public async Task<IActionResult> CreateDisbursement([FromBody] CreateDisbursementDto dto)
        {
            _logger.LogInformation("API: Creating disbursement for LoanId: {LoanId}", dto.LoanId);
            var result = await _disbursementService.CreateDisbursementAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Borrower,Admin")]
        [HttpGet("disbursement/by-loan")]
        public async Task<IActionResult> GetDisbursementsByLoanId([FromQuery] string loanId)
        {
            _logger.LogInformation("API: Fetching disbursements for LoanId: {LoanId}", loanId);
            var result = await _disbursementService.GetDisbursementsByLoanIdAsync(loanId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Borrower,Admin")]
        [HttpGet("disbursement/get-by-id")]
        public async Task<IActionResult> GetDisbursementById([FromQuery] string id)
        {
            _logger.LogInformation("API: Fetching disbursement by Id: {DisbursementId}", id);
            var result = await _disbursementService.GetDisbursementByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Borrower,Admin")]
        [HttpPost("fundusage/log-usage")]
        public async Task<IActionResult> LogFundUsage([FromBody] CreateFundUsageDto dto)
        {
            _logger.LogInformation("API: Logging fund usage for DisbursementId: {DisbursementId}", dto.DisbursementId);
            var result = await _disbursementService.LogFundUsageAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Borrower,Admin")]
        [HttpGet("fundusage/usages")]
        public async Task<IActionResult> GetFundUsagesByDisbursementId([FromQuery] string disbursementId)
        {
            _logger.LogInformation("API: Fetching fund usages for DisbursementId: {DisbursementId}", disbursementId);
            var result = await _disbursementService.GetFundUsagesByDisbursementIdAsync(disbursementId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
    }
} 