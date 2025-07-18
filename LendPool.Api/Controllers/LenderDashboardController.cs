using System.Security.Claims;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Lender")]
    public class LenderDashboardController : BaseController
    {
        private readonly ILenderDashboardService _service;
        private readonly ILogger<LenderDashboardController> _logger;
        public LenderDashboardController(ILenderDashboardService service, ILogger<LenderDashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string GetLenderId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("pool/loans/funded")]
        public async Task<IActionResult> GetFundedLoans()
        {
            var lenderId = GetLenderId();
            _logger.LogInformation("API: GetFundedLoans called for lender {LenderId}", lenderId);
            var result = await _service.GetFundedLoansAsync(lenderId);
            _logger.LogInformation($"API: GetFundedLoans returned {result.Data.Count} loans for lender {lenderId}");
            return Ok(result.Data);
        }

        [HttpGet("pool/repayments/earnings")]
        public async Task<IActionResult> GetRepaymentsAndEarnings()
        {
            var lenderId = GetLenderId();
            _logger.LogInformation("API: GetRepaymentsAndEarnings called for lender {LenderId}", lenderId);
            var result = await _service.GetRepaymentsAndEarningsAsync(lenderId);
            _logger.LogInformation("API: GetRepaymentsAndEarnings returned status {Status} for lender {LenderId}", result.StatusCode, lenderId);
            return Ok(result);
        }

        [HttpGet("pool/performance")]
        public async Task<IActionResult> GetPoolPerformance()
        {
            var lenderId = GetLenderId();
            _logger.LogInformation("API: GetPoolPerformance called for lender {LenderId}", lenderId);
            var result = await _service.GetPoolPerformanceAsync(lenderId);
            _logger.LogInformation("API: GetPoolPerformance returned status {Status} for lender {LenderId}", result.StatusCode, lenderId);
            return Ok(result);
        }
    }
} 