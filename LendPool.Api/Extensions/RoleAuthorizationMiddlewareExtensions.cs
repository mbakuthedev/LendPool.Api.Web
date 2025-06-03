using LendPool.Api.Middlewares;

namespace LendPool.Api.Extensions
{
    public static class RoleAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleAuthorization(this IApplicationBuilder builder, params string[] roles)
        {
            return builder.UseMiddleware<RoleAuthorizationMiddleware>((object)roles);
        }
    }


}
