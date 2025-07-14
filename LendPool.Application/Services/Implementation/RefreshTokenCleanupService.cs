using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LendPool.Application.Services.Implementation
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RefreshTokenCleanupService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24); // Run once per day

        public RefreshTokenCleanupService(IServiceProvider serviceProvider, ILogger<RefreshTokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var refreshTokenRepo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                    
                    await refreshTokenRepo.DeleteExpiredTokensAsync();
                    _logger.LogInformation("Expired refresh tokens cleaned up successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired refresh tokens");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
} 