 A permission matrix to handle the permissions manually

 {
  "RolePermissions": {
    "Admin": [
      "Permissions.ViewPools",
      "Permissions.ManageUsers",
      "Permissions.JoinLenderPool",
      "Permissions.RequestLoan"
    ],
    "Lender": [
      "Permissions.ViewPools",
      "Permissions.JoinLenderPool"
    ],
    "Borrower": [
      "Permissions.RequestLoan"
    ]
  }
}

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IConfiguration _config;

    public PermissionHandler(IConfiguration config)
    {
        _config = config;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(role)) return Task.CompletedTask;

        var rolePermissions = _config.GetSection("RolePermissions").Get<Dictionary<string, List<string>>>();

        if (rolePermissions != null &&
            rolePermissions.TryGetValue(role, out var permissions) &&
            permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}


public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });

    services.AddScoped<IAuthorizationHandler, PermissionHandler>();

    services.AddAuthorization(options =>
    {
        var permissions = configuration.GetSection("RolePermissions")
                                       .Get<Dictionary<string, List<string>>>()
                                       ?.SelectMany(kvp => kvp.Value)
                                       .Distinct();

        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement(permission));
                });
            }
        }

        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    return services;
}

[Authorize(Policy = "Permissions.ViewPools")]
[HttpGet("view-pools")]
public IActionResult ViewPools()
{
    return Ok("Accessible by Admin and Lender");
}
builder.Configuration.AddJsonFile("permissions.json", optional: false, reloadOnChange: true);


| Scenario                                                    | Recommendation                                                    |
| ----------------------------------------------------------- | ----------------------------------------------------------------- |
| A user can be **both lender and borrower**                  | Use **multi-role assignment** (many-to-many UserRoles table)      |
| Roles can change after registration                         | Implement a **role management system** (admin editable)           |
| Role behaviors depend on **context (e.g., per-pool roles)** | Use **scoped roles**, like `SuperLender` for `PoolX`              |
| Fine-grained permissions needed                             | Implement **claims-based authorization** or **permissions table** |



Thinking about making it a b2b platform, where you look for pools with lower interests , borrow there and then invest into your own pool and get you ROI, then repay your loan

Borrower requests loan from pool -> it goes through an approval matrix -> after approval, loan is created -> borrower can repay loan -> lender can withdraw interest and principal

