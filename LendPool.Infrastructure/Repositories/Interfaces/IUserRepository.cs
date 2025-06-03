using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<GenericResponse<User>> UpdateUserAsync(User user);
        Task<GenericResponse<User>> GetUserByIdAsync(string id);
        Task<GenericResponse<User>> AddUserAsync(User user);
        Task<GenericResponse<User>> GetUserByEmailAsync(string email);
    }
}
