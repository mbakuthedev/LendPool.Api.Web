using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IInterestPaymentService
    {
        Task<GenericResponse<bool>> RecordInterestPaymentAsync(string poolId, string loanId, string userId, decimal amount);
        Task<decimal> GetTotalInterestEarnedAsync(string poolId);
        Task<List<InterestPayment>> GetInterestPaymentsByPoolAsync(string poolId);
    }

}
