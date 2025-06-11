using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LendPool.Api.Extensions
{
    public static class SwaggerExtension
    {

            public static IServiceCollection AddSwaggerWithJwtSupport(this IServiceCollection services)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                    // JWT Authentication in Swagger
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token like: Bearer {your token}"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                });

                return services;
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

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            //  //  options.AddPolicy("LenderOnly", policy => policy.RequireRole("Lender"));
            //    options.AddPolicy("BorrowerOnly", policy => policy.RequireRole("Borrower"));
            //});



            return services;
        }


        public static IServiceCollection AddCustomAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Lender", policy =>
                    policy.RequireRole("Lender", "Admin"));

                options.AddPolicy("Borrower", policy =>
                    policy.RequireRole("Borrower", "Admin"));

                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));
            });

            return services;
        }

    }
}