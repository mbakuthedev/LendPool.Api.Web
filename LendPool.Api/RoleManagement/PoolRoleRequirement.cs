using Microsoft.AspNetCore.Authorization;

namespace LendPool.Api.RoleManagement
{
    public class PoolRoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; }
        public PoolRoleRequirement(string role) => Role = role;
    }
}
