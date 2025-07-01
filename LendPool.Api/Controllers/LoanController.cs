using LendPool.Application.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LendPool.Domain.DTOs;
using LendPool.Application.Services.Implementation;
using Microsoft.AspNetCore.Authorization;

namespace LendPool.Api.Controllers
{
  
        [ApiController]
    
        public class LoanController : BaseController
        {
            private readonly ILoanService _loanService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ILenderpoolService _lenderPoolService;

            public LoanController(ILoanService loanService, IHttpContextAccessor httpContextAccessor, ILenderpoolService lenderpoolService)
            {
                _loanService = loanService;
            _lenderPoolService = lenderpoolService;
                _httpContextAccessor = httpContextAccessor;
            }

            [HttpGet("borrower/get-all-pools")]
            public async Task<IActionResult> GetAllPools()
            {
                var pools = await _lenderPoolService.GetAllPoolsAsync();
                return Ok(pools);
            }

        private string GetUserId() =>
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            [HttpPost("loan/request-loan")]
        public async Task<IActionResult> RequestLoan([FromBody] LoanRequestDto dto)
        {
            var userId = GetUserId(); 
            var result = await _loanService.SubmitLoanRequestAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet("loan/my-requests")]
            public async Task<IActionResult> MyLoanRequests()
            {
                var result = await _loanService.GetLoanRequestsByUserAsync(GetUserId());
                return Ok(result);
            }

        [Authorize(Roles = "Lender")]
        [HttpPut("loan/approve-loan")]
        public async Task<IActionResult> ApproveLoan([FromBody] ApproveLoanRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid approval input");

            var lenderId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(lenderId))
                return Unauthorized("Lender not authenticated");

            var result = await _loanService.ApproveLoanAsync(lenderId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
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

            [HttpGet("loan/get-loan-by-id")]
            public async Task<IActionResult> GetLoan([FromQuery]string id)
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                return Ok(loan);
            }

            [HttpGet("loan/get-loans-by-pool-id")]
            public async Task<IActionResult> GetLoansByPool([FromQuery]string id)
            {
                var loans = await _loanService.GetLoansByPoolAsync(id);
                return Ok(loans);
            }
        }

    }

