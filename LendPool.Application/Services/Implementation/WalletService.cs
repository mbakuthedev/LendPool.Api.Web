using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Models;
using LendPool.Infrastructure.Repositories.Interfaces;
using System.Transactions;
using LendPool.Domain.Enums;
using LendPool.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository walletRepository, ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

           
        public async Task<GenericResponse<Wallet>> GetWalletByUserIdAsync(string userId)
        {
            var user =  await _walletRepository.GetWalletByUserIdAsync(userId);

            if (user == null)
            {
                return GenericResponse<Wallet>.FailResponse("Wallet not found for the specified user.", 404);
            }

            return GenericResponse<Wallet>.SuccessResponse(user,200,"Wallet user gotten sucesfully");
        }


        public async Task<bool> CreditAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            _logger.LogInformation("Crediting user {UserId} with {Amount}", userId, amount);
            
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null) 
            {
                _logger.LogWarning("Wallet not found for user {UserId}", userId);
                return false;
            }

            var originalBalance = wallet.Balance;
            wallet.Balance += amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Credit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Wallet credit"
            };

            try
            {
                // Step 1: Update wallet balance
                var walletUpdated = await _walletRepository.UpdateWalletAsync(wallet);
                if (!walletUpdated)
                {
                    _logger.LogError("Failed to update wallet balance for user {UserId}", userId);
                    return false;
                }

                // Step 2: Add transaction record
                var transactionAdded = await _walletRepository.AddTransactionAsync(transaction);
                if (!transactionAdded)
                {
                    _logger.LogError("Failed to add transaction record for user {UserId}, rolling back wallet balance", userId);
                    // Rollback wallet balance
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                    return false;
                }

                _logger.LogInformation("Successfully credited user {UserId} with {Amount}", userId, amount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during credit operation for user {UserId}", userId);
                // Rollback wallet balance if exception occurs
                try
                {
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback wallet balance for user {UserId}", userId);
                }
                return false;
            }
        }

        public async Task<bool> DebitAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            _logger.LogInformation("Debiting user {UserId} with {Amount}", userId, amount);
            
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount) 
            {
                _logger.LogWarning("Insufficient funds or wallet not found for user {UserId}. Balance: {Balance}, Requested: {Amount}", 
                    userId, wallet?.Balance ?? 0, amount);
                return false;
            }

            var originalBalance = wallet.Balance;
            wallet.Balance -= amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Debit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Wallet debit"
            };

            try
            {
                // Step 1: Update wallet balance
                var walletUpdated = await _walletRepository.UpdateWalletAsync(wallet);
                if (!walletUpdated)
                {
                    _logger.LogError("Failed to update wallet balance for user {UserId}", userId);
                    return false;
                }

                // Step 2: Add transaction record
                var transactionAdded = await _walletRepository.AddTransactionAsync(transaction);
                if (!transactionAdded)
                {
                    _logger.LogError("Failed to add transaction record for user {UserId}, rolling back wallet balance", userId);
                    // Rollback wallet balance
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                    return false;
                }

                _logger.LogInformation("Successfully debited user {UserId} with {Amount}", userId, amount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during debit operation for user {UserId}", userId);
                // Rollback wallet balance if exception occurs
                try
                {
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback wallet balance for user {UserId}", userId);
                }
                return false;
            }
        }

        public async Task<decimal> GetBalanceAsync(string userId)
        {
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            return wallet?.Balance ?? 0;
        }

        public async Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string reference = null)
        {
            _logger.LogInformation("Initiating transfer of {Amount} from user {FromUserId} to user {ToUserId}", amount, fromUserId, toUserId);
            
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var debitSuccess = await DebitAsync(fromUserId, amount, reference, $"Transfer to {toUserId}");
                if (!debitSuccess)
                {
                    _logger.LogError("Failed to debit user {FromUserId} during transfer", fromUserId);
                    return false;
                }

                var creditSuccess = await CreditAsync(toUserId, amount, reference, $"Transfer from {fromUserId}");
                if (!creditSuccess)
                {
                    _logger.LogError("Failed to credit user {ToUserId} during transfer, transaction will be rolled back", toUserId);
                    return false;
                }

                scope.Complete();
                _logger.LogInformation("Successfully completed transfer of {Amount} from user {FromUserId} to user {ToUserId}", amount, fromUserId, toUserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during transfer operation from user {FromUserId} to user {ToUserId}", fromUserId, toUserId);
                return false;
            }
        }


        public async Task<GenericResponse<Wallet>> CreateWalletAsync(string userId)
        {
            var existing = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (existing != null)
                return GenericResponse<Wallet>.FailResponse("Wallet already exists", 400);

            var wallet = new Wallet { UserId = userId, Balance = 0, Currency = "NGN" };
            var result = await _walletRepository.AddWalletAsync(wallet);

            if (!result) return GenericResponse<Wallet>.FailResponse("Failed to create wallet", 500);
            return GenericResponse<Wallet>.SuccessResponse(wallet, 201);
        }

        public async Task<GenericResponse<List<WalletTransaction>>> GetAllTransactionsAsync()
        {
            var txns = await _walletRepository.GetAllTransactionsAsync();
            return GenericResponse<List<WalletTransaction>>.SuccessResponse(txns, 200);
        }

        public async Task<GenericResponse<WalletTransaction>> GetTransactionByIdAsync(string transactionId)
        {
            var txn = await _walletRepository.GetTransactionByIdAsync(transactionId);
            if (txn == null) return GenericResponse<WalletTransaction>.FailResponse("Transaction not found", 404);
            return GenericResponse<WalletTransaction>.SuccessResponse(txn, 200);
        }

        //move to admin service later
        public async Task<GenericResponse<bool>> AdminCreditAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            _logger.LogInformation("Admin crediting user {UserId} with {Amount}", userId, amount);
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null) return GenericResponse<bool>.FailResponse("Wallet not found", 404);

            var originalBalance = wallet.Balance;
            wallet.Balance += amount;
            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Credit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Admin credit"
            };

            try
            {
                // Step 1: Update wallet balance
                var walletUpdated = await _walletRepository.UpdateWalletAsync(wallet);
                if (!walletUpdated)
                {
                    _logger.LogError("Failed to update wallet balance for admin credit operation for user {UserId}", userId);
                    return GenericResponse<bool>.FailResponse("Failed to update wallet balance", 500);
                }

                // Step 2: Add transaction record
                var transactionAdded = await _walletRepository.AddTransactionAsync(txn);
                if (!transactionAdded)
                {
                    _logger.LogError("Failed to add transaction record for admin credit operation for user {UserId}, rolling back wallet balance", userId);
                    // Rollback wallet balance
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                    return GenericResponse<bool>.FailResponse("Failed to add transaction record", 500);
                }

                _logger.LogInformation("Successfully completed admin credit for user {UserId} with {Amount}", userId, amount);
                return GenericResponse<bool>.SuccessResponse(true, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin credit operation for user {UserId}", userId);
                // Rollback wallet balance if exception occurs
                try
                {
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback wallet balance for admin credit operation for user {UserId}", userId);
                }
                return GenericResponse<bool>.FailResponse("Transaction failed", 500);
            }
        }

        public async Task<GenericResponse<bool>> AdminDebitAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            _logger.LogInformation("Admin debiting user {UserId} with {Amount}", userId, amount);
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount) return GenericResponse<bool>.FailResponse("Insufficient funds or wallet not found", 400);

            var originalBalance = wallet.Balance;
            wallet.Balance -= amount;
            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Debit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Admin debit"
            };

            try
            {
                // Step 1: Update wallet balance
                var walletUpdated = await _walletRepository.UpdateWalletAsync(wallet);
                if (!walletUpdated)
                {
                    _logger.LogError("Failed to update wallet balance for admin debit operation for user {UserId}", userId);
                    return GenericResponse<bool>.FailResponse("Failed to update wallet balance", 500);
                }

                // Step 2: Add transaction record
                var transactionAdded = await _walletRepository.AddTransactionAsync(txn);
                if (!transactionAdded)
                {
                    _logger.LogError("Failed to add transaction record for admin debit operation for user {UserId}, rolling back wallet balance", userId);
                    // Rollback wallet balance
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                    return GenericResponse<bool>.FailResponse("Failed to add transaction record", 500);
                }

                _logger.LogInformation("Successfully completed admin debit for user {UserId} with {Amount}", userId, amount);
                return GenericResponse<bool>.SuccessResponse(true, 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin debit operation for user {UserId}", userId);
                // Rollback wallet balance if exception occurs
                try
                {
                    wallet.Balance = originalBalance;
                    await _walletRepository.UpdateWalletAsync(wallet);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback wallet balance for admin debit operation for user {UserId}", userId);
                }
                return GenericResponse<bool>.FailResponse("Transaction failed", 500);
            }
        }

        public async Task<GenericResponse<List<WalletTransaction>>> GetTransactionsAsync(string userId)
        {
            var txns = await _walletRepository.GetTransactionsByUserAsync(userId);
            return GenericResponse<List<WalletTransaction>>.SuccessResponse(txns, 200);
        }

    }

}
