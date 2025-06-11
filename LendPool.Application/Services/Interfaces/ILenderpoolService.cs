using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.DTOs;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface ILenderpoolService
    {
        Task<GenericResponse<string>> AddUserToPoolAsync(AddUserToPoolDto dto, string actingUserId);
   
        Task<GenericResponse<LenderPool>> CreateLenderPoolAsync(CreateLenderPoolDto dto, string creatorUserId);
        Task<GenericResponse<IEnumerable<LenderPool>>> GetAllPoolsAsync();
        Task<GenericResponse<IEnumerable<LoanDto>>> GetActiveLoansByPoolAsync(string poolId);

        Task<GenericResponse<bool>> ContributeToPoolAsync(ContributeToPoolDto contributeToPoolDto);
        Task<GenericResponse<PoolSummaryDto>> GetPoolSummaryAsync(string poolId);
        Task<GenericResponse<bool>> WithdrawFromPoolAsync(WithdrawFromPoolDto withdrawFromPoolDto);
      //  Task<GenericResponse<string>> AddUserToPoolAsync(AddUserToPoolDto dto);

    }
}
