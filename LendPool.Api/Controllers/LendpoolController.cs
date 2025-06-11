using LendPool.Domain.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LendPool.Api.Controllers
{
    [Authorize("Lender")]
    public class LendpoolController : BaseController
    {
        private readonly ILenderpoolService _lenderPoolService;

        public LendpoolController(ILenderpoolService lenderpoolService)
        {
            _lenderPoolService = lenderpoolService;
        }


        

        [HttpPost("lender/create-pool")]
        public async Task<IActionResult> CreatePool([FromBody] CreateLenderPoolDto dto)
        {
            var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pool = await _lenderPoolService.CreateLenderPoolAsync(dto, userId);
            return Ok(pool);
        }

        [HttpPost("lender/add-user")]
        public async Task<IActionResult> AddUserToPool([FromBody] AddUserToPoolDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pool = await _lenderPoolService.AddUserToPoolAsync(dto, userId);
            return Ok(pool);
        }


        [HttpGet("lender/get-all-pools")]
        public async Task<IActionResult> GetAllPools()
        {
            var pools = await _lenderPoolService.GetAllPoolsAsync();
            return Ok(pools);
        }

        [HttpPost("lender/contribute/{poolId}")]
        public async Task<IActionResult> Contribute(ContributeToPoolDto contributeToPoolDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _lenderPoolService.ContributeToPoolAsync(contributeToPoolDto);
            return result.Data ? Ok("Contribution successful") : BadRequest("Failed to contribute");
        }

        [HttpGet("lender/loans/{poolId}")]
        public async Task<IActionResult> GetActiveLoans(string poolId)
        {
            var loans = await _lenderPoolService.GetActiveLoansByPoolAsync(poolId);
            return Ok(loans);
        }

        [HttpGet("lender/summary/{poolId}")]
        public async Task<IActionResult> GetPoolSummary(string poolId)
        {
            var summary = await _lenderPoolService.GetPoolSummaryAsync(poolId);
            return Ok(summary);
        }

        [HttpPost("lender/withdraw/{poolId}")]
        public async Task<IActionResult> Withdraw(WithdrawFromPoolDto withdrawFromPoolDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _lenderPoolService.WithdrawFromPoolAsync(withdrawFromPoolDto);
            return result.Data ? Ok("Withdrawal successful") : BadRequest("Failed to withdraw");
        }
    }
}
