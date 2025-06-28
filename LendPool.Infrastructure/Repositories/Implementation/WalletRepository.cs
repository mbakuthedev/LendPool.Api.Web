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

        public async Task<List<WalletTransaction>> GetTransactionsByUserAsync(string userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            return wallet?.Transactions.OrderByDescending(t => t.Timestamp).ToList() ?? new List<WalletTransaction>();
        }

        public async Task<WalletTransaction> GetTransactionByIdAsync(string transactionId)
        {
            return await _context.WalletTransactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<List<Wallet>> GetAllWalletsAsync()
        {
            return await _context.Wallets
                .Include(w => w.User)
                .Include(w => w.Transactions)
                .ToListAsync();
        }

        public async Task<List<WalletTransaction>> GetAllTransactionsAsync()
        {
            return await _context.WalletTransactions
                .Include(t => t.Wallet)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> AddWalletAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
