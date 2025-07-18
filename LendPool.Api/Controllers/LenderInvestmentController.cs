using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LenderInvestmentController : ControllerBase
    {
        private readonly ILenderInvestmentService _lenderInvestmentService;
        private readonly ILogger<LenderInvestmentController> _logger;

        public LenderInvestmentController(ILenderInvestmentService lenderInvestmentService, ILogger<LenderInvestmentController> logger)
        {
            _lenderInvestmentService = lenderInvestmentService;
            _logger = logger;
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateInvestment([FromBody] CreateLenderInvestmentDto dto)
        {
            _logger.LogInformation("API: Creating investment for LenderId: {LenderId}, PoolId: {PoolId}", dto.LenderId, dto.PoolId);
            var result = await _lenderInvestmentService.CreateInvestmentAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawInvestment([FromBody] WithdrawInvestmentDto dto)
        {
            _logger.LogInformation("API: Processing withdrawal for InvestmentId: {InvestmentId}", dto.InvestmentId);
            var result = await _lenderInvestmentService.WithdrawInvestmentAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("investment/{investmentId}")]
        public async Task<IActionResult> GetInvestmentById([FromRoute] string investmentId)
        {
            _logger.LogInformation("API: Fetching investment by Id: {InvestmentId}", investmentId);
            var result = await _lenderInvestmentService.GetInvestmentByIdAsync(investmentId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("pool/{poolId}")]
        public async Task<IActionResult> GetInvestmentsByPoolId([FromRoute] string poolId)
        {
            _logger.LogInformation("API: Fetching investments for PoolId: {PoolId}", poolId);
            var result = await _lenderInvestmentService.GetInvestmentsByPoolIdAsync(poolId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("lender/{lenderId}")]
        public async Task<IActionResult> GetInvestmentsByLenderId([FromRoute] string lenderId)
        {
            _logger.LogInformation("API: Fetching investments for LenderId: {LenderId}", lenderId);
            var result = await _lenderInvestmentService.GetInvestmentsByLenderIdAsync(lenderId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("profit-share/{investmentId}")]
        public async Task<IActionResult> CalculateProfitShare([FromRoute] string investmentId)
        {
            _logger.LogInformation("API: Calculating profit share for InvestmentId: {InvestmentId}", investmentId);
            var result = await _lenderInvestmentService.CalculateProfitShareAsync(investmentId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("loss-share/{investmentId}")]
        public async Task<IActionResult> CalculateLossShare([FromRoute] string investmentId)
        {
            _logger.LogInformation("API: Calculating loss share for InvestmentId: {InvestmentId}", investmentId);
            var result = await _lenderInvestmentService.CalculateLossShareAsync(investmentId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("early-withdrawal-penalty/{investmentId}")]
        public async Task<IActionResult> CalculateEarlyWithdrawalPenalty([FromRoute] string investmentId)
        {
            _logger.LogInformation("API: Calculating early withdrawal penalty for InvestmentId: {InvestmentId}", investmentId);
            var result = await _lenderInvestmentService.CalculateEarlyWithdrawalPenaltyAsync(investmentId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Lender,Admin")]
        [HttpGet("pool-tenor/{poolId}")]
        public async Task<IActionResult> GetPoolTenor([FromRoute] string poolId)
        {
            _logger.LogInformation("API: Fetching pool tenor for PoolId: {PoolId}", poolId);
            var result = await _lenderInvestmentService.GetPoolTenorAsync(poolId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("pool-tenor/{poolId}/status")]
        public async Task<IActionResult> UpdatePoolTenorStatus([FromRoute] string poolId, [FromBody] string status)
        {
            _logger.LogInformation("API: Updating pool tenor status for PoolId: {PoolId} to {Status}", poolId, status);
            var result = await _lenderInvestmentService.UpdatePoolTenorStatusAsync(poolId, status);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 