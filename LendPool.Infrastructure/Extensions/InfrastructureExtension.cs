using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Infrastructure.Repositories.Implementation;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LendPool.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionStrin = Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");

            var connectionString = config.GetConnectionString("RenderConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    x => x.MigrationsAssembly("LendPool.Infrastructure")
                )
            );


            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Core Repositories
            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IWalletRepository, WalletRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
            // Pool Repositories
            services.AddScoped<ILendpoolRepository, LendpoolRepository>()
                .AddScoped<ILenderInvestmentRepository, LenderInvestmentRepository>()
                .AddScoped<IPoolTenorRepository, PoolTenorRepository>();
            // services.AddScoped<ILenderMembershipRepository, LenderMembershipRepository>(); // Interface not found
            
            // Loan Repositories
            services.AddScoped<ILoanRepository, LoanRepository>()
                .AddScoped<IDisbursementRepository, DisbursementRepository>()
                .AddScoped<IRepaymentRepository, RepaymentRepository>();
            
            // System Repositories
            services.AddScoped<IVotingRepository, VotingRepository>()
                .AddScoped<IReconciliationRepository, ReconciliationRepository>();
             
            return services;
        }
    }
}
