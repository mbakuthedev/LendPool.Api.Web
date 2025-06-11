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
    }

}
