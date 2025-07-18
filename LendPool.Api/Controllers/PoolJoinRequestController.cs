using System.Security.Claims;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LendPool.Api.Controllers
{
    [ApiController]
    public class PoolJoinRequestController : BaseController
    {
        private readonly ILenderPoolJoinRequestService _service;
        private readonly ILogger<PoolJoinRequestController> _logger;
        public PoolJoinRequestController(ILenderPoolJoinRequestService service, ILogger<PoolJoinRequestController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Lender: Send join request
        [HttpPost("pool/send-join-request")]
        [Authorize(Roles = "Lender")]
        public async Task<IActionResult> SendJoinRequest([FromBody] CreateJoinRequestDto dto)
        {
            var userId = GetUserId();
            _logger.LogInformation("API: SendJoinRequest called by lender {LenderId} for pool {PoolId}", userId, dto.PoolId);
            var result = await _service.SendJoinRequestAsync(userId, dto);
            _logger.LogInformation("API: SendJoinRequest result for lender {LenderId} and pool {PoolId}: {Status}", userId, dto.PoolId, result.StatusCode);
            return Ok(result);
        }

        // SuperLender: List join requests for a pool
        [HttpGet("pool/get-join-requests")]
        [Authorize(Roles = "SuperLender,Admin")]
        public async Task<IActionResult> GetJoinRequests(string poolId)
        {
            _logger.LogInformation("API: GetJoinRequests called for pool {PoolId}", poolId);
            var result = await _service.GetJoinRequestsForPoolAsync(poolId);
            _logger.LogInformation("API: GetJoinRequests for pool {PoolId} returned {Count} requests", poolId, result.Data?.Count ?? 0);
            return Ok(result);
        }

        // SuperLender: Review (accept/reject) a join request
        [HttpPost("pool/join-requests-{requestId}/review")]
        [Authorize(Roles = "SuperLender,Admin")]
        public async Task<IActionResult> ReviewJoinRequest(string poolId, string requestId, [FromBody] ReviewJoinRequestDto dto)
        {
            var userId = GetUserId();
            _logger.LogInformation("API: ReviewJoinRequest called by superlender {SuperLenderId} for request {RequestId}", userId, requestId);
            var success = await _service.ReviewJoinRequestAsync(userId, requestId, dto);
            _logger.LogInformation("API: ReviewJoinRequest for request {RequestId} by {SuperLenderId} result: {Status}", requestId, userId, success.StatusCode);
            if (success.Success)
            {
                return BadRequest(new { success = false, message = success.Message });
            }
            return success.Data ? Ok(new { success = true }) : BadRequest(new { success = false });
        }
    }
} 