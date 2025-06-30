using LendPool.Domain.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

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
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid input data.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User is not authenticated.");

            var result = await _lenderPoolService.CreateLenderPoolAsync(dto, userId);

            if (!result.Success)
                return BadRequestResponse(result.Message);

            return Success(result);
        }

        [HttpGet("lender/get-pool-by-id")]
        public async Task<IActionResult> GetPoolById(string poolId)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User is not authenticated.");

            var result = await _lenderPoolService.GetPoolById(poolId);

            if (!result.Success)
                return BadRequestResponse(result.Message);

            return Success(result);
        }


        [Authorize(Policy = "SuperLenderPolicy")]
        [HttpPost("lender/{poolId}/add-user")]
        public async Task<IActionResult> AddUserToPool(string poolId, [FromBody] AddUserToPoolDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dto.PoolId = poolId;
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
