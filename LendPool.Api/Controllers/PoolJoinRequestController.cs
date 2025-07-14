using System.Security.Claims;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    [ApiController]
    public class PoolJoinRequestController : BaseController
    {
        private readonly ILenderPoolJoinRequestService _service;
        public PoolJoinRequestController(ILenderPoolJoinRequestService service)
        {
            _service = service;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Lender: Send join request
        [HttpPost("pool/send-join-request")]
        [Authorize(Roles = "Lender")]
        public async Task<IActionResult> SendJoinRequest([FromBody] CreateJoinRequestDto dto)
        {
          
            var result = await _service.SendJoinRequestAsync(GetUserId(), dto);
            return Ok(result);
        }

        // SuperLender: List join requests for a pool
        [HttpGet("pool/get-join-requests")]
        [Authorize(Roles = "SuperLender,Admin")]
        public async Task<IActionResult> GetJoinRequests(string poolId)
        {
            var result = await _service.GetJoinRequestsForPoolAsync(poolId);
            return Ok(result);
        }

        // SuperLender: Review (accept/reject) a join request
        [HttpPost("pool/join-requests-{requestId}/review")]
        [Authorize(Roles = "SuperLender,Admin")]
        public async Task<IActionResult> ReviewJoinRequest(string poolId, string requestId, [FromBody] ReviewJoinRequestDto dto)
        {
            var success = await _service.ReviewJoinRequestAsync(GetUserId(), requestId, dto);
            if (success.Success)
            {
                return BadRequest(new { success = false, message = success.Message });
            }
            return success.Data ? Ok(new { success = true }) : BadRequest(new { success = false });
        }
    }
} 