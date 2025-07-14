using System.Security.Claims;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Borrower")]
    public class BorrowerAnalyticsController : BaseController
    {
        private readonly IBorrowerAnalyticsService _analyticsService;
        public BorrowerAnalyticsController(IBorrowerAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("borrower/analytics/total-borrowed")]
        public async Task<IActionResult> GetTotalBorrowed() => Ok(new { totalBorrowed = await _analyticsService.GetTotalBorrowedAsync(GetUserId()) });

        [HttpGet("borrower/analytics/current-loan")]
        public async Task<IActionResult> GetCurrentLoan() => Ok(new { currentLoan = await _analyticsService.GetCurrentLoanAsync(GetUserId()) });

        [HttpGet("borrower/analytics/outstanding-balance")]
        public async Task<IActionResult> GetOutstandingBalance() => Ok(new { outstandingBalance = await _analyticsService.GetOutstandingBalanceAsync(GetUserId()) });

        [HttpGet("borrower/analytics/next-payment")]
        public async Task<IActionResult> GetNextPayment() => Ok(new { nextPayment = await _analyticsService.GetNextPaymentDateAsync(GetUserId()) });

        [HttpGet("borrower/analytics/loan-by-month")]
        public async Task<IActionResult> GetLoanByMonth() => Ok(new { data = await _analyticsService.GetLoanByMonthAsync(GetUserId()) });

        [HttpGet("borrower/analytics/repayments-by-month")]
        public async Task<IActionResult> GetRepaymentsByMonth() => Ok(new { data = await _analyticsService.GetRepaymentsByMonthAsync(GetUserId()) });

        [HttpGet("borrower/analytics/repayment-history")]
        public async Task<IActionResult> GetRepaymentHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _analyticsService.GetRepaymentHistoryAsync(GetUserId(), page, pageSize));
    }
} 