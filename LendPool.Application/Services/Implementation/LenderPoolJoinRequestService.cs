using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.Data;
using LendPool.Domain.Enums;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;
using LendPool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LendPool.Application.Services.Implementation
{
    public class LenderPoolJoinRequestService : ILenderPoolJoinRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILendpoolRepository _poolRepo;

        public LenderPoolJoinRequestService(ApplicationDbContext db, ILendpoolRepository poolRepo)
        {
            _db = db;
            _poolRepo = poolRepo;
        }

        public async Task<GenericResponse<LenderPoolJoinRequestDto>> SendJoinRequestAsync(string lenderId, CreateJoinRequestDto dto)
        {
            // 1. Check if already a member
            if (await _poolRepo.IsUserInPoolAsync(lenderId, dto.PoolId))
            {
                return GenericResponse<LenderPoolJoinRequestDto>.FailResponse("You are already a member of this pool.", 400);
            }
               

            // 2. Must accept rules
            if (!dto.AcceptedRules)
                return GenericResponse<LenderPoolJoinRequestDto>.FailResponse("You must accept the pool rules to join.");

            // 3. Prevent duplicate pending requests
            var existing = await _db.Set<LenderPoolJoinRequest>()
                .FirstOrDefaultAsync(r => r.PoolId == dto.PoolId && r.LenderId == lenderId && r.Status == JoinRequestStatus.Pending.ToString());
            if (existing != null)
                return GenericResponse<LenderPoolJoinRequestDto>.FailResponse("You already have a pending join request for this pool.");

            // 4. Create join request
            var request = new LenderPoolJoinRequest
            {
                Id = Guid.NewGuid().ToString(),
                PoolId = dto.PoolId,
                LenderId = lenderId,
                Status = JoinRequestStatus.Pending.ToString(),
                RequestedAt = DateTime.UtcNow
            };
            _db.LenderPoolJoinRequests.Add(request);
            await _db.SaveChangesAsync();

            var newRequest =  new LenderPoolJoinRequestDto
            {
                Id = request.Id,
                PoolId = request.PoolId,
                LenderId = request.LenderId,
                Status = request.Status,
                RequestedAt = request.RequestedAt
            };

            return GenericResponse<LenderPoolJoinRequestDto>.SuccessResponse(newRequest, 200, $"Sucessfully created a request to join pool with {newRequest.PoolId}");
        }

        public async Task<GenericResponse<List<LenderPoolJoinRequestDto>>> GetJoinRequestsForPoolAsync(string poolId)
        {
            var requests = await _db.LenderPoolJoinRequests.Where(r => r.PoolId == poolId)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            var joinRequests =  requests.Select(r => new LenderPoolJoinRequestDto
            {
                Id = r.Id,
                PoolId = r.PoolId,
                LenderId = r.LenderId,
                Status = r.Status,
                RequestedAt = r.RequestedAt,
                ReviewedAt = r.ReviewedAt,
                ReviewedBy = r.ReviewedBy
            }).ToList();

            return GenericResponse<List<LenderPoolJoinRequestDto>>.SuccessResponse(joinRequests, 200, " join requests retrieved successfully.");
        }

        public async Task<GenericResponse<bool>> ReviewJoinRequestAsync(string superLenderId, string requestId, ReviewJoinRequestDto dto)
        {
            var request = await _db.LenderPoolJoinRequests.Include(r => r.Pool).FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null) return GenericResponse<bool>.FailResponse("No request found",404);
            if (request.Status != JoinRequestStatus.Pending.ToString()) return GenericResponse<bool>.FailResponse("You have already joined this pool", 404);

            // Only the pool creator (superlender) can review
            if (request.Pool.CreatedByUserId != superLenderId)
                return GenericResponse<bool>.FailResponse("Only the pool creator can review join requests.");

            request.Status = dto.Accept ? JoinRequestStatus.Accepted.ToString() : JoinRequestStatus.Rejected.ToString();
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedBy = superLenderId;

            if (dto.Accept)
            {
                // Always add as Lender (except creator, who is already SuperLender)
                var membership = new LenderPoolMembership
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.LenderId,
                    LenderPoolId = request.PoolId,
                    JoinedAt = DateTime.UtcNow,
                    Role = UserRole.Lender.ToString() // Default role for new members
                };
                _db.LenderPoolMemberships.Add(membership);
            }

            await _db.SaveChangesAsync();
            return GenericResponse<bool>.SuccessResponse(true, 201, "You have joined the pool sucessfully");
        }
    }
} 