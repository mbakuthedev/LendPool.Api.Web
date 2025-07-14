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
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("auth/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("auth/revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RevokeTokenAsync(request.RefreshToken);
                return Ok(result);
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

        [Authorize(Roles = "Borrower")]
        [HttpPost("user/update-kyc-borrower")]
        public async Task<IActionResult> UpdateBorrowerKyc([FromBody] BorrowerKycUpdateRequest request)
        {
          
            var resultData = await _authService.UpdateBorrowerKycAsync(request);
            var result = resultData.Data;

            if (!result) return BadRequest("Invalid user or not a borrower");

            return Ok("Borrower KYC updated");
        }

        [HttpGet("user/me")]
        [Authorize]

        public IActionResult GetProfile()
        {
            var fullName = User.FindFirst("FullName")?.Value;
            return Ok(new { FullName = fullName });
        }


        [Authorize(Roles = "Lender")]
        [HttpPost("user/kyc-lender")]
        public async Task<IActionResult> UpdateLenderKyc([FromBody] LenderKycUpdateRequest request)
        {
            var resultData = await _authService.UpdateLenderKycAsync(request);

            if (!resultData.Success || !resultData.Data)
                return BadRequest(resultData.Message);

            return Ok(resultData.Message);
        }

        [HttpPost("auth/register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
