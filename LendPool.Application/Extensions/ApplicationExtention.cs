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
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBorrowerAnalyticsService, BorrowerAnalyticsService>();
            services.AddScoped<IAdminLoanMatchingService, AdminLoanMatchingService>();
            services.AddScoped<ILenderDashboardService, LenderDashboardService>();
            services.AddScoped<ILenderPoolJoinRequestService, LenderPoolJoinRequestService>();
            // Add other application services here
            return services;
        }
    }
}
