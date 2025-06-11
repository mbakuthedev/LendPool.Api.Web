using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetWalletByUserIdAsync(string userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<bool> UpdateWalletAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddTransactionAsync(WalletTransaction transaction)
        {
            await _context.WalletTransactions.AddAsync(transaction);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
