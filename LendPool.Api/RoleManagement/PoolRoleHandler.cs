using System.Security.Claims;
using LendPool.Domain.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Api.RoleManagement
{
    public class PoolRoleHandler : AuthorizationHandler<PoolRoleRequirement>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PoolRoleHandler(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, PoolRoleRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var poolId = _httpContextAccessor.HttpContext?.GetRouteValue("poolId")?.ToString();


            var membership = await _dbContext.LenderPoolMemberships
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.LenderPoolId == poolId);

            if (membership != null && membership.Role == requirement.Role)
            {
                context.Succeed(requirement);
            }

        }
    }
}
