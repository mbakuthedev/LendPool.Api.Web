using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminLoanMatchingController : BaseController
    {
        private readonly IAdminLoanMatchingService _service;
        public AdminLoanMatchingController(IAdminLoanMatchingService service)
        {
            _service = service;
        }

        [HttpGet("admin/loan/unmatched")]
        public async Task<IActionResult> GetUnmatchedLoanRequests()
            => Ok(await _service.GetUnmatchedLoanRequestsAsync());

        [HttpGet("admin/loan/pools")]
        public async Task<IActionResult> GetLenderPools()
            => Ok(await _service.GetLenderPoolsAsync());

        [HttpPost("admin/loan/{loanRequestId}/assign-pool")]
        public async Task<IActionResult> AssignPoolToLoanRequest(string loanRequestId, [FromBody] AssignPoolRequestDto dto)
        {
            var result = await _service.AssignPoolToLoanRequestAsync(loanRequestId, dto.PoolId);
            return result.Data ? Ok(new { success = true }) : BadRequest(new { success = false });
        }
    }
} 