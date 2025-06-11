using System.Security.Claims;

namespace LendPool.Api.Middlewares
{


        //public class RoleAuthorizationMiddleware
        //{
        //    private readonly RequestDelegate _next;
        //    private readonly string[] _requiredRoles;

        //    public RoleAuthorizationMiddleware(RequestDelegate next, params string[] requiredRoles)
        //    {
        //        _next = next;
        //        _requiredRoles = requiredRoles;
        //    }

        //    public async Task InvokeAsync(HttpContext context)
        //    {
        //        if (!context.User.Identity?.IsAuthenticated ?? false)
        //        {
        //            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //            await context.Response.WriteAsync("Unauthorized");
        //            return;
        //        }

        //        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        //        if (string.IsNullOrWhiteSpace(roleClaim) || !_requiredRoles.Contains(roleClaim, StringComparer.OrdinalIgnoreCase))
        //        {
        //            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //            await context.Response.WriteAsync("Forbidden: Insufficient role");
        //            return;
        //        }

        //        await _next(context);
        //    }
        //}

    }

