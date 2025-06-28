using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IWalletService
    {
        Task<GenericResponse<Wallet>> GetWalletByUserIdAsync(string userId);
        Task<bool> CreditAsync(string userId, decimal amount, string reference = null, string description = null);
        Task<bool> DebitAsync(string userId, decimal amount, string reference = null, string description = null);
        Task<decimal> GetBalanceAsync(string userId);
        Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string reference = null);
        Task<GenericResponse<Wallet>> CreateWalletAsync(string userId);
        Task<GenericResponse<List<WalletTransaction>>> GetTransactionsAsync(string userId);
        Task<GenericResponse<List<WalletTransaction>>> GetAllTransactionsAsync();
        Task<GenericResponse<WalletTransaction>> GetTransactionByIdAsync(string transactionId);
        Task<GenericResponse<bool>> AdminCreditAsync(string userId, decimal amount, string reference = null, string description = null);
        Task<GenericResponse<bool>> AdminDebitAsync(string userId, decimal amount, string reference = null, string description = null);
    }

}
