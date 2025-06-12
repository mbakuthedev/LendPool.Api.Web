using System.Security.Claims;
using LendPool.Application.Requests;
using LendPool.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
 
   
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAsync(request);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("user/current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUser();
                return Ok(new { user });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("kyc-borrower")]
        public async Task<IActionResult> UpdateBorrowerKyc([FromBody] BorrowerKycUpdateRequest request)
        {
          
            var resultData = await _authService.UpdateBorrowerKycAsync(request);
            var result = resultData.Data;

            if (!result) return BadRequest("Invalid user or not a borrower");

            return Ok("Borrower KYC updated");
        }

        [Authorize(Roles = "Lender")]
        [HttpPost("kyc-lender")]
        public async Task<IActionResult> UpdateLenderKyc([FromBody] LenderKycUpdateRequest request)
        {
            var resultData = await _authService.UpdateLenderKycAsync(request);

            if (!resultData.Success || !resultData.Data)
                return BadRequest(resultData.Message);

            return Ok(resultData.Message);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
