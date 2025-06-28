using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Infrastructure.Repositories.Interfaces
{
    public interface ILenderMembershipRepository
    {
        Task<GenericResponse<PaginatedResponse<LenderPool>>> GetAllPoolsWithMembersAsync(int pageNumber, int pageSize);
        Task<GenericResponse<PaginatedResponse<LenderPool>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<GenericResponse<IEnumerable<UserDto>>> GetUsersInPoolAsync(Guid poolId);
        Task<GenericResponse<string>> AddUserToPoolAsync(LenderPoolMembership membership);
        Task<GenericResponse<string>> RemoveUserFromPoolAsync(string userId, Guid poolId);
        Task<GenericResponse<string>> UpdateUserRoleAsync(string userId, Guid poolId, string newRole);
        Task<bool> IsUserInPoolAsync(string userId, Guid poolId);
        Task<string?> GetUserRoleInPoolAsync(string userId, Guid poolId);
    }
}
