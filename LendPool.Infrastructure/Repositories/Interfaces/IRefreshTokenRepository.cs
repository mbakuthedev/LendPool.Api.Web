using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<GenericResponse<RefreshToken>> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> RevokeAllUserTokensAsync(string userId);
        Task<bool> DeleteExpiredTokensAsync();
    }
} 