using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LendPool.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Lender")]
    public class VotingController : BaseController
    {
        private readonly IVotingService _votingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VotingController(IVotingService votingService, IHttpContextAccessor httpContextAccessor)
        {
            _votingService = votingService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId() =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPost("cast-vote")]
        public async Task<IActionResult> CastVote([FromBody] CastVoteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid vote input");

            var lenderId = GetUserId();
            if (string.IsNullOrWhiteSpace(lenderId))
                return Unauthorized("Lender not authenticated");

            var result = await _votingService.CastVoteAsync(lenderId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("vote-result/{operationId}/{operationType}")]
        public async Task<IActionResult> GetVoteResult(string operationId, VoteOperationType operationType)
        {
            var result = await _votingService.GetVoteResultAsync(operationId, operationType);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("pool-votes/{poolId}")]
        public async Task<IActionResult> GetPoolVoteResults(string poolId)
        {
            var result = await _votingService.GetVoteResultsByPoolAsync(poolId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("pool-summary/{poolId}")]
        public async Task<IActionResult> GetPoolVotingSummary(string poolId)
        {
            var result = await _votingService.GetPoolVotingSummaryAsync(poolId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("has-voted/{operationId}/{operationType}")]
        public async Task<IActionResult> HasVoted(string operationId, VoteOperationType operationType)
        {
            var lenderId = GetUserId();
            if (string.IsNullOrWhiteSpace(lenderId))
                return Unauthorized("Lender not authenticated");

            var result = await _votingService.HasVotedAsync(lenderId, operationId, operationType);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("pool-member-count/{poolId}")]
        public async Task<IActionResult> GetPoolMemberCount(string poolId)
        {
            var result = await _votingService.GetPoolMemberCountAsync(poolId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("active-voter-count/{operationId}/{operationType}")]
        public async Task<IActionResult> GetActiveVoterCount(string operationId, VoteOperationType operationType)
        {
            var result = await _votingService.GetActiveVoterCountAsync(operationId, operationType);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
} 