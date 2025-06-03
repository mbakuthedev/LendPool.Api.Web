using LendPool.Domain.Data;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<User>> AddUserAsync(User user)
        {
            if (user == null)
            {
                return GenericResponse<User>.FailResponse("User cannot be null", 400);
            }

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return GenericResponse<User>.SuccessResponse(user, 201, "User created successfully");
        }
        public async Task<GenericResponse<User>> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return GenericResponse<User>.SuccessResponse(user, 200, "User updated successfully");
        }

        public async Task<GenericResponse<User>> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return GenericResponse<User>.FailResponse("User not found");

            return GenericResponse<User>.SuccessResponse(user,200,"User fetched successfully");
        }


        public async Task<GenericResponse<User>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new GenericResponse<User>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = null
                    };
                }

                return new GenericResponse<User>
                {
                    Success = true,
                    Message = "User found",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<User>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

    }
}
