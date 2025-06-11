using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Models;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace LendPool.Application.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepository)
        {
            _userRepo = userRepository;
        }
        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            return user.Data;
        }
    }
}
