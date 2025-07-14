using System.Collections.Generic;
using System.Threading.Tasks;
using LendPool.Application.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILenderPoolJoinRequestService
    {
        Task<GenericResponse<LenderPoolJoinRequestDto>> SendJoinRequestAsync(string lenderId, CreateJoinRequestDto dto);
        Task<GenericResponse<List<LenderPoolJoinRequestDto>>> GetJoinRequestsForPoolAsync(string poolId);
        Task<GenericResponse<bool>> ReviewJoinRequestAsync(string superLenderId, string requestId, ReviewJoinRequestDto dto);
    }
} 