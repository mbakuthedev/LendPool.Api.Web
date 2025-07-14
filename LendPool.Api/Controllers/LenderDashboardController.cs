using System.Security.Claims;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Lender")]
    public class LenderDashboardController : BaseController
    {
        private readonly ILenderDashboardService _service;
        public LenderDashboardController(ILenderDashboardService service)
        {
            _service = service;
        }

        private string GetLenderId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("pool/loans/funded")]
        public async Task<IActionResult> GetFundedLoans() => Ok(await _service.GetFundedLoansAsync(GetLenderId()));

        [HttpGet("pool/repayments/earnings")]
        public async Task<IActionResult> GetRepaymentsAndEarnings() => Ok(await _service.GetRepaymentsAndEarningsAsync(GetLenderId()));

        [HttpGet("pool/performance")]
        public async Task<IActionResult> GetPoolPerformance() => Ok(await _service.GetPoolPerformanceAsync(GetLenderId()));
    }
} 