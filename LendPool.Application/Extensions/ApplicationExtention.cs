using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Implementation;
using LendPool.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LendPool.Application.Extensions
{
    public static class ApplicationExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Core Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
            
            // Loan Services
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IDisbursementService, DisbursementService>();
            services.AddScoped<IRepaymentService, RepaymentService>();
            services.AddScoped<IInterestPaymentService, InterestPaymentService>();
            
            // Pool Services
            services.AddScoped<ILenderpoolService, LenderpoolService>();
            services.AddScoped<ILenderInvestmentService, LenderInvestmentService>();
            // services.AddScoped<ILenderMembershipService, LenderMembershipService>(); // Implementation not found
            
            // Request Services
            services.AddScoped<ILenderPoolJoinRequestService, LenderPoolJoinRequestService>();
            
            // Dashboard & Analytics
            services.AddScoped<ILenderDashboardService, LenderDashboardService>();
            services.AddScoped<IBorrowerAnalyticsService, BorrowerAnalyticsService>();
            
            // Voting & Reconciliation
            services.AddScoped<IVotingService, VotingService>();
            services.AddScoped<IReconciliationService, ReconciliationService>();
            
            // Admin Services
            services.AddScoped<IAdminLoanMatchingService, AdminLoanMatchingService>();
            
            // Background Services (registered as hosted services in Program.cs)
            // services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();
            
            return services;
        }
    }
}
