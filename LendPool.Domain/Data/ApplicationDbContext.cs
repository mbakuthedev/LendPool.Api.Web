using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Domain.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
       // public DbSet<Lender> Lenders { get; set; }
        public DbSet<LenderPool> LenderPools { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanRequest> LoanRequests { get; set; }
        public DbSet<PoolContribution> PoolContributions { get; set; }
        public DbSet<Repayment> Repayments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<PoolWithdrawal> PoolWithdrawals { get; set; }
        public DbSet<InterestPayment> InterestPayments { get; set; }
        public DbSet<LenderPoolMembership> LenderPoolMemberships { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<User>().HasData(new User
            {

                Id = "11111111-1111-1111-1111-111111111111",
                DateCreated = new DateTime(2023, 1, 1),
                FirstName = "Admin",
                LastName = "Gbemidebe",
                DateModified = new DateTime(2023, 1, 1),
                Email = "admin@lendpool.com",
                PasswordHash = "$2a$11$.7M3DWXSh2PA6ETF4DyBLuKwDj1SAY7.aEfcj3a7x7q8ClPMT42bO", // Hashed password for "Admin@123"
                Role = UserRole.Admin.ToString(),
                FullName = "Admin",
                IsKycVerified = false,
                DocumentId = null,
                DocumentUrl = null


            });
        }

    }
}
