using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.DTOs;
using LendPool.Domain.Responses;

namespace LendPool.Application.Services.Interfaces
{
    public interface IRepaymentService
    {
        Task<GenericResponse<IEnumerable<RepaymentDto>>> GetRepaymentsAndEarningsAsync(string poolId);
        decimal CalculateInterestEarned(decimal amountPaid, decimal interestRate);
        decimal CalculateLateFee(DateTime dueDate, DateTime paymentDate, decimal baseAmount);
        decimal CalculateRemainingLoanBalance(decimal totalLoanAmount, decimal totalRepaid);
    }

}
