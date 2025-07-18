using System;
using LendPool.Domain.Enums;

namespace LendPool.Application.DTOs
{
    public class LenderInvestmentDto
    {
        public string Id { get; set; }
        public string LenderId { get; set; }
        public string PoolId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal InvestmentPercentage { get; set; }
        public DateTime InvestmentDate { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public decimal ProfitEarned { get; set; }
        public decimal LossIncurred { get; set; }
        public string Status { get; set; }
        public string WithdrawalReason { get; set; }
        public bool IsEarlyWithdrawal { get; set; }
        public decimal EarlyWithdrawalPenalty { get; set; }
    }

    public class CreateLenderInvestmentDto
    {
        public string LenderId { get; set; }
        public string PoolId { get; set; }
        public decimal InvestmentAmount { get; set; }
    }

    public class WithdrawInvestmentDto
    {
        public string InvestmentId { get; set; }
        public string WithdrawalReason { get; set; }
        public decimal WithdrawalAmount { get; set; }
    }

    public class PoolTenorDto
    {
        public string Id { get; set; }
        public string PoolId { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalPoolAmount { get; set; }
        public decimal TotalLentAmount { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalLoss { get; set; }
        public string Status { get; set; }
    }
} 