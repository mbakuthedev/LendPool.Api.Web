using LendPool.Api.Extensions;
using LendPool.Application.Extensions;
using LendPool.Domain.Data;
using LendPool.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCorsPolicy();


builder.Services.AddControllers();

builder.Services.AddSwaggerWithJwtSupport()
    .AddCustomAuthorizationPolicies()
    .AddJwtAuthentication(builder.Configuration);


builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

//app.UseWhen(
//    context => context.Request.Path.StartsWithSegments("/api/secured"),
//    appBuilder => appBuilder.UseRoleAuthorization("Lender", "Admin")
//);


//app.UseWhen(
//    context => context.Request.Path.StartsWithSegments("/api/borrower"),
//    appBuilder => appBuilder.UseRoleAuthorization("Borrower")
//);

app.MapControllers();

app.Run();
