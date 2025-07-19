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

            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ILendpoolRepository, LendpoolRepository>()
                .AddScoped<IRepaymentRepository, RepaymentRepository>()
                .AddScoped<ILoanRepository, LoanRepository>()
                .AddScoped<IWalletRepository, WalletRepository>()
                .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddScoped<IVotingRepository, VotingRepository>();
               // .AddScoped<>;
             
            return services;
        }
    }
}
