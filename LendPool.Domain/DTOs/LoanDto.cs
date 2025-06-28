using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;

namespace LendPool.Domain.DTOs
{
    public class LoanDto
    {
        public string PoolId { get; set; }
        public string UserId { get; set; }
        public decimal TotalRepaid { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string LoanStatus { get; set; }
        public bool IsActive { get; set; }

    }

    public class CreateLoanDto
    {
        public decimal Amount { get; set; }
        public decimal TotalRepaid { get; set; }
        public string UserId { get; set; }
        public string PoolId { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string LoanStatus { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoanRequestDto
    {
        public decimal RequestedAmount { get; set; }
        public string Purpose { get; set; }
        public string DurationInMonths { get; set; }
        public string RequestStatus { get; set; }
        public int TenureInDays { get; set; }
        public string MatchedPoolId { get; set; }
    }


    public class RepaymentDto
    {
        public string RepaymentId { get; set; }
        public string UserId { get; set; }
        public string LoanId { get; set; }
        public string PoolId { get; set; }

        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal InterestEarned { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal RemainingLoanBalance { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsFullyRepaid { get; set; }
        public bool IsLate { get; set; }
        public decimal LateFee { get; set; }

        public string TransactionReference { get; set; }
        public string PaymentChannel { get; set; }
    }

   
}