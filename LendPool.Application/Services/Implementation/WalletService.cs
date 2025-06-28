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
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null) return false;

            wallet.Balance += amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Credit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Wallet credit"
            };

            return await _walletRepository.UpdateWalletAsync(wallet) &&
                   await _walletRepository.AddTransactionAsync(transaction);
        }

        public async Task<bool> DebitAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount) return false;

            wallet.Balance -= amount;

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Debit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Wallet debit"
            };

            return await _walletRepository.UpdateWalletAsync(wallet) &&
                   await _walletRepository.AddTransactionAsync(transaction);
        }

        public async Task<decimal> GetBalanceAsync(string userId)
        {
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            return wallet?.Balance ?? 0;
        }

        public async Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string reference = null)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var debitSuccess = await DebitAsync(fromUserId, amount, reference, $"Transfer to {toUserId}");
            if (!debitSuccess) return false;

            var creditSuccess = await CreditAsync(toUserId, amount, reference, $"Transfer from {fromUserId}");
            if (!creditSuccess) return false;

            scope.Complete();
            return true;
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

            wallet.Balance += amount;
            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Credit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Admin credit"
            };

            var updated = await _walletRepository.UpdateWalletAsync(wallet) && await _walletRepository.AddTransactionAsync(txn);
            return updated ? GenericResponse<bool>.SuccessResponse(true, 200) : GenericResponse<bool>.FailResponse("Transaction failed", 500);
        }

        public async Task<GenericResponse<bool>> AdminDebitAsync(string userId, decimal amount, string reference = null, string description = null)
        {
            _logger.LogInformation("Admin debiting user {UserId} with {Amount}", userId, amount);
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount) return GenericResponse<bool>.FailResponse("Insufficient funds or wallet not found", 400);

            wallet.Balance -= amount;
            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Debit.ToString(),
                Reference = reference ?? Guid.NewGuid().ToString(),
                Description = description ?? "Admin debit"
            };

            var updated = await _walletRepository.UpdateWalletAsync(wallet) && await _walletRepository.AddTransactionAsync(txn);
            return updated ? GenericResponse<bool>.SuccessResponse(true, 200) : GenericResponse<bool>.FailResponse("Transaction failed", 500);
        }

        public async Task<GenericResponse<List<WalletTransaction>>> GetTransactionsAsync(string userId)
        {
            var txns = await _walletRepository.GetTransactionsByUserAsync(userId);
            return GenericResponse<List<WalletTransaction>>.SuccessResponse(txns, 200);
        }

    }

}
