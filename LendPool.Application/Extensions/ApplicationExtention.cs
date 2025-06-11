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
            services.AddScoped<IAuthService, AuthService>()
                .AddScoped<ILenderpoolService, LenderpoolService>()
                .AddScoped<IInterestPaymentService, InterestPaymentService>()
                .AddScoped<IRepaymentService, RepaymentService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IWalletService,WalletService>();
                //.AddScoped<IInterestPaymentService, Inrwe>;

            return services;
        }
    }
}
