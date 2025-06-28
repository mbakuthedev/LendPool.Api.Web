using LendPool.Application.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LendPool.Domain.DTOs;

namespace LendPool.Api.Controllers
{
  
        [ApiController]
    
        public class LoanController : BaseController
        {
            private readonly ILoanService _loanService;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public LoanController(ILoanService loanService, IHttpContextAccessor httpContextAccessor)
            {
                _loanService = loanService;
                _httpContextAccessor = httpContextAccessor;
            }

            private string GetUserId() =>
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            [HttpPost("request-loan")]
            public async Task<IActionResult> RequestLoan([FromBody] LoanRequestDto dto)
            {
                await _loanService.SubmitLoanRequestAsync(GetUserId(), dto);
                return Ok("Loan request submitted.");
            }

            [HttpGet("loan/my-requests")]
            public async Task<IActionResult> MyLoanRequests()
            {
                var result = await _loanService.GetLoanRequestsByUserAsync(GetUserId());
                return Ok(result);
            }

            [HttpPut("loan/{id}/approve-loan")]
            public async Task<IActionResult> ApproveLoan(string id, [FromQuery] string poolId, [FromBody] string comment)
            {
                await _loanService.ApproveLoanAsync(id, poolId, comment);
                return Ok("Loan approved.");
            }

            //[HttpPut("{id}/reject")]
            //public async Task<IActionResult> RejectLoan(Guid id, [FromBody] string reason)
            //{
            //    await _loanService.RejectLoanRequestAsync(id, reason);
            //    return Ok("Loan rejected.");
            //}

            [HttpGet("loan/my-loans")]
            public async Task<IActionResult> MyLoans()
            {
                var loans = await _loanService.GetLoansByUserAsync(GetUserId());
                return Ok(loans);
            }

            [HttpGet("loan/{id}")]
            public async Task<IActionResult> GetLoan(string id)
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                return Ok(loan);
            }

            [HttpGet("loan/pool/{id}")]
            public async Task<IActionResult> GetLoansByPool(string id)
            {
                var loans = await _loanService.GetLoansByPoolAsync(id);
                return Ok(loans);
            }
        }

    }

