using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IWalletService _walletService;
        public AdminController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/credit")]
        public async Task<IActionResult> AdminCredit([FromBody] AdminWalletDto dto)
        {
            var result = await _walletService.AdminCreditAsync(dto.UserId, dto.Amount, dto.Reference, dto.Description);
            return result.Success ? Success(result) : BadRequestResponse(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/debit")]
        public async Task<IActionResult> AdminDebit([FromBody] AdminWalletDto dto)
        {
            var result = await _walletService.AdminDebitAsync(dto.UserId, dto.Amount, dto.Reference, dto.Description);
            return result.Success ? Success(result) : BadRequestResponse(result.Message);
        }
    }
}
