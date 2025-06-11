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

namespace LendPool.Application.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
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
    }

}
