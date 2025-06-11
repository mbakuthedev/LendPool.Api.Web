using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> GetWalletByUserIdAsync(string userId);
        Task<bool> UpdateWalletAsync(Wallet wallet);
        Task<bool> AddTransactionAsync(WalletTransaction transaction);
    }

}
