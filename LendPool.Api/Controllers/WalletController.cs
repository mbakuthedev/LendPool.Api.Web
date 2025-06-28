using LendPool.Domain.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LendPool.Application.Services.Interfaces;

namespace LendPool.Api.Controllers
{
    [ApiController]
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _logger = logger;
            _walletService = walletService;
        }


        [HttpGet("wallet/me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Fetching wallet for user {UserId}", userId);

            var response = await _walletService.GetWalletByUserIdAsync(userId);
            if (!response.Success)
            {
                _logger.LogWarning("Wallet not found for user {UserId}", userId);
                return NotFoundResponse(response.Message);
            }

            return Success(response);
        }

        [HttpGet("wallet/balance")]
        public async Task<IActionResult> GetMyBalance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Fetching wallet balance for user {UserId}", userId);

            var balance = await _walletService.GetBalanceAsync(userId);
            return Success(new { Balance = balance });
        }

        [HttpPost("wallet/credit")]
        public async Task<IActionResult> CreditMyWallet([FromQuery] decimal amount, [FromQuery] string reference = null, [FromQuery] string description = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Attempting to credit wallet for user {UserId} with amount {Amount}", userId, amount);

            var result = await _walletService.CreditAsync(userId, amount, reference, description);
            if (!result)
            {
                _logger.LogWarning("Failed to credit wallet for user {UserId}", userId);
                return BadRequestResponse("Failed to credit wallet.");
            }

            return Success("Wallet credited successfully.");
        }

        [HttpPost("wallet/debit")]
        public async Task<IActionResult> DebitMyWallet([FromQuery] decimal amount, [FromQuery] string reference = null, [FromQuery] string description = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Attempting to debit wallet for user {UserId} with amount {Amount}", userId, amount);

            var result = await _walletService.DebitAsync(userId, amount, reference, description);
            if (!result)
            {
                _logger.LogWarning("Failed to debit wallet or insufficient balance for user {UserId}", userId);
                return BadRequestResponse("Failed to debit wallet or insufficient balance.");
            }

            return Success("Wallet debited successfully.");
        }

        [HttpPost("wallet/transfer")]
        public async Task<IActionResult> Transfer([FromQuery] string toUserId, [FromQuery] decimal amount, [FromQuery] string reference = null)
        {
            var fromUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("User {FromUserId} attempting transfer of {Amount} to user {ToUserId}", fromUserId, amount, toUserId);

            var result = await _walletService.TransferAsync(fromUserId, toUserId, amount, reference);
            if (!result)
            {
                _logger.LogWarning("Transfer from user {FromUserId} to user {ToUserId} failed", fromUserId, toUserId);
                return BadRequestResponse("Transfer failed.");
            }

            return Success("Transfer successful.");
        }

        [HttpPost("wallet/create")]
        public async Task<IActionResult> CreateWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _walletService.CreateWalletAsync(userId);
            return result.Success ? Success(result) : BadRequestResponse(result.Message);
        }


        [HttpGet("wallet/transactions")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _walletService.GetTransactionsAsync(userId);
            return Success(result);
        }

        [HttpGet("wallet/transactions/{transactionId}")]
        public async Task<IActionResult> GetTransaction(string transactionId)
        {
            var result = await _walletService.GetTransactionByIdAsync(transactionId);
            return result.Success ? Success(result) : NotFoundResponse(result.Message);
        }

    }

}

