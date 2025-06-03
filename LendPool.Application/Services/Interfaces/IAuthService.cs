using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Application.Requests;
using LendPool.Application.Responses;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<GenericResponse<bool>> UpdateBorrowerKycAsync(BorrowerKycUpdateRequest request);
        Task<GenericResponse<bool>> UpdateLenderKycAsync(LenderKycUpdateRequest request);
        Task<GenericResponse<string>> RegisterAsync(RegisterRequest request);
        Task<GenericResponse<AuthResponse>> LoginAsync(LoginRequest request);

    }
}
