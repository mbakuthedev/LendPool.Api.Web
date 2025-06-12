using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Requests;
using LendPool.Application.Responses;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Implementation;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LendPool.Application.Services.Implementation
{   public class AuthService : IAuthService
    {

        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserRepository repo, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<GenericResponse<CurrentUserDto>> GetCurrentUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                {
                    return GenericResponse<CurrentUserDto>.FailResponse("User is not authenticated", 401);
                }

                var user = httpContext.User;

                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                            ?? user.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return GenericResponse<CurrentUserDto>.FailResponse("User ID not found in token", 401);
                }

                var authResponse = new CurrentUserDto
                {
                    UserId = userId,
                    Email = email,
                    Role = role,
                  //  Message = "User authenticated"
                };

                return GenericResponse<CurrentUserDto>.SuccessResponse(authResponse, 200, "User gotten sucessfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<CurrentUserDto>.FailResponse($"An error occurred: {ex.Message}", 500);
            }
        }


        public async Task<GenericResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var userResponse = await _repo.GetUserByEmailAsync(request.Email);
                var user = userResponse.Data;

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return GenericResponse<AuthResponse>.FailResponse("Invalid credentials", 401);
                }

                var token = GenerateJwtToken(user);

                var authResponse = new AuthResponse
                {
                    Token = token,
                 
                };

                return GenericResponse<AuthResponse>.SuccessResponse(authResponse, 200, "Login successful");
            }
            catch (Exception ex)
            {
                return GenericResponse<AuthResponse>.FailResponse($"An error occurred: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _repo.GetUserByEmailAsync(request.Email);
            if (existingUser.Data != null)
            {
                return GenericResponse<string>.FailResponse("Email already registered", 400);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                FullName = request.Fullname,
                PasswordHash = hashedPassword,
                Role = request.Role
            };

            await _repo.AddUserAsync(user);

            return GenericResponse<string>.SuccessResponse("User registered successfully", 201);
        }

        public async Task<GenericResponse<bool>> UpdateBorrowerKycAsync(BorrowerKycUpdateRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidId))
                return GenericResponse<bool>.FailResponse("Invalid or missing user ID");

            var userData = await _repo.GetUserByIdAsync(userId);
            var user = userData.Data;
            if (user == null || user.Role != UserRole.Borrower.ToString()) return GenericResponse<bool>.FailResponse("User has to be a borrower", 400);

            user.BVN = request.BVN;
            user.NIN = request.NationalId;
            user.DateOfBirth = request.DateOfBirth;

            await _repo.UpdateUserAsync(user);
            return GenericResponse<bool>.SuccessResponse(true,200, "Borrower information updated successfully");
        }

        public async Task<GenericResponse<bool>> UpdateLenderKycAsync(LenderKycUpdateRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userId))
                return GenericResponse<bool>.FailResponse("Invalid or missing user ID");

            var userData = await _repo.GetUserByIdAsync(userId);
            var user = userData.Data;

            if (user == null || user.Role != UserRole.Lender.ToString())
                return GenericResponse<bool>.FailResponse("User has to be a Lender");

            user.BusinessName = request.BusinessName;
            user.RegistrationNumber = request.RegistrationNumber;
            user.InvestmentCapacity = request.InvestmentCapacity;

            await _repo.UpdateUserAsync(user);

            return GenericResponse<bool>.SuccessResponse(true, 200, "Lender information updated successfully");
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }
}
