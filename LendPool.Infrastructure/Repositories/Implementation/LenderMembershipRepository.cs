using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Data;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Infrastructure.Repositories.Implementation
{
    //public class LenderMembershipRepository : ILenderMembershipRepository
    //{
    //    private readonly ApplicationDbContext _context;

    //    public LenderMembershipRepository(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<GenericResponse<PaginatedResponse<LenderPool>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    //    {
    //        var query = _context.LenderPools
    //            .Include(p => p.LenderPoolMemberships)
    //                .ThenInclude(m => m.User) // assuming you have a navigation property to User
    //            .AsQueryable();

    //        var totalCount = await query.CountAsync();

    //        var pools = await query
    //            .Skip((pageNumber - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToListAsync();

    //        if (!pools.Any())
    //        {
    //            return GenericResponse<PaginatedResponse<LenderPool>>.FailResponse("No pools found", 404);
    //        }

    //        var result = new PaginatedResponse<LenderPool>
    //        {
    //            Data = pools,
    //            TotalCount = totalCount,
    //            PageSize = pageSize,
    //            CurrentPage = pageNumber
    //        };

    //        return GenericResponse<PaginatedResponse<LenderPool>>.SuccessResponse(result, 200);
    //    }

    //    public async Task<GenericResponse<IEnumerable<UserDto>>> GetUsersInPoolAsync(string poolId)
    //    {
    //        var members = await _context.LenderPoolMemberships
    //            .Include(m => m.User)
    //            .Where(m => m.LenderPoolId == poolId)
    //            .Select(m => new UserDto
    //            {
    //                UserId = m.UserId,
    //                Email = m.User.Email,
    //                FullName = m.User.FullName,
    //                Role = m.Role
    //            })
    //            .ToListAsync();

    //        return GenericResponse<IEnumerable<UserDto>>.SuccessResponse(members, 200);
    //    }

      

    //    public async Task<GenericResponse<string>> RemoveUserFromPoolAsync(string userId, string poolId)
    //    {
    //        var membership = await _context.LenderPoolMemberships
    //            .FirstOrDefaultAsync(m => m.UserId == userId && m.LenderPoolId == poolId);

    //        if (membership == null)
    //            return GenericResponse<string>.FailResponse("Membership not found", 404);

    //        _context.LenderPoolMemberships.Remove(membership);
    //        await _context.SaveChangesAsync();
    //        return GenericResponse<string>.SuccessResponse("User removed", 200);
    //    }

    //    public async Task<GenericResponse<string>> UpdateUserRoleAsync(string userId, string poolId, string newRole)
    //    {
    //        var membership = await _context.LenderPoolMemberships
    //            .FirstOrDefaultAsync(m => m.UserId == userId && m.LenderPoolId == poolId);

    //        if (membership == null)
    //            return GenericResponse<string>.FailResponse("Membership not found", 404);

    //        membership.Role = newRole;
    //        await _context.SaveChangesAsync();
    //        return GenericResponse<string>.SuccessResponse("User role updated", 200);
    //    }

    //    public async Task<bool> IsUserInPoolAsync(string userId, string poolId)
    //    {
    //        return await _context.LenderPoolMemberships
    //            .AnyAsync(m => m.UserId == userId && m.LenderPoolId == poolId);
    //    }

    //    public async Task<string?> GetUserRoleInPoolAsync(string userId, string poolId)
    //    {
    //        return await _context.LenderPoolMemberships
    //            .Where(m => m.UserId == userId && m.LenderPoolId == poolId)
    //            .Select(m => m.Role)
    //            .FirstOrDefaultAsync();
    //    }


    //}
}
